using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using UnityEngine.Assertions;

namespace SAS.Async
{
	//Routines do triple duty as task-likes, task-builders, and awaiters in order to keep internal access/pooling easy
	public abstract partial class Routine : INotifyCompletion
	{
		/// <summary> Enable stack tracing for debugging. Off by default due to performance implications. </summary>
		public static bool TracingEnabled { get; set; }
		/// <summary> The running instance id of the routine. If the routine is stopped, will return zero. </summary>
		public ulong Id { get { return _id; } }
		/// <summary> Indicates if routine is stopped. </summary>
		public bool IsDead { get { return _id == 0; } }
		/// <summary> Internal use only. Required for awaiter. </summary>
		public bool IsCompleted { get { return _state == State.Finished; } }

		protected enum State
		{
			NotStarted,
			Running,
			Finished
		}

		internal protected interface IStateMachineRef
		{
			void MoveNext();
		}

		internal protected class StateMachineRef<T> : IStateMachineRef where T : IAsyncStateMachine
		{
			public T value;
			public void MoveNext() { value.MoveNext(); }
		}

		protected ulong _id = 0; //Used to verify a routine is still the same instance and hasn't been recycled
		protected State _state = State.NotStarted;
		protected bool _stopChildrenOnStep = false; //Kill children when stepping. Used by WaitForAny
		protected IStateMachineRef _stateMachine = null; //The generated state machine for the async method
		protected AsyncTaskManager _manager = null; //The manager to use for WaitForNextFrame
		protected Routine _parent = null; //Routine that spawned this one
		protected readonly List<Routine> _children = new List<Routine>(); //Routines spawned by this one
		protected Action _onFinish = null; //Continuation to call when async method is finished
		protected Action<Exception> _onStop = null;
		protected Exception _thrownException = null;
		protected readonly Action _stepAction;
		protected Action _stepAnyAction;
		protected Action _stepAllAction;
		//Top-most routine currently being stepped
		protected static Routine Current { get { return (steppingStack.Count > 0) ? steppingStack.Peek() : null; } }
		
		private static UInt64 nextId = 1; //Id generator. 64bits should be enough, right?
		//Tracks actively stepping routines
		private static readonly Stack<Routine> steppingStack = new Stack<Routine>();
		//Is routine active?
		private bool IsRunning { get { return !IsDead && !IsCompleted; } }

		public Routine()
		{
			_stepAction = MoveNext;
		}

		/// <summary> Stop the routine. </summary>
		public void Stop()
		{
			_id = 0;
			ReleaseChildren();
			_stopChildrenOnStep = false;
			_onFinish = null;

			if (_onStop != null)
			{
				_onStop(_thrownException);
				_onStop = null;
			}
		}

		/// <summary> Internal use only. Executes to the next await or end of the async method. </summary>
		public void MoveNext()
		{
			if (IsDead)
				return;

			//First step
			if (_state == State.NotStarted)
				_state = State.Running;

			//Step async method to the next await
			if (_stateMachine != null)
			{
				//Stop children, but don't release them because their result might be needed
				if (_stopChildrenOnStep)
				{
					foreach (var child in _children)
						child.Stop();
				}

				var currentId = _id;
				steppingStack.Push(this);
				_stateMachine.MoveNext();
				Assert.IsTrue(steppingStack.Peek() == this);
				steppingStack.Pop();
				if (currentId != _id)
					return;

				//Now we can release dead children back to the pool
				for (var i = 0; i < _children.Count;)
				{
					var child = _children[i];
					if (child.IsDead)
					{
						_children.RemoveAt(i);
						child.Release();
					}
					else
						++i;
				}
			}
			else
			{
				//Routine was not an async method
				ReleaseChildren();
				_state = State.Finished;
			}

			//All done: resume parent if needed
			if (_state == State.Finished)
				Finish();
		}

		/// <summary> Internal use only. Receives continuation to call when async method finishes. </summary>
		public void OnCompleted(Action continuation)
		{
			_onFinish = continuation;
		}

		/// <summary> Internal use only. Throw an exception into the routine. </summary>
		public void Throw(Exception exception)
		{
			var awaitingRoutines = RoutinePool.GetAwaiterList();
			CollectAwaitingRoutines(this, awaitingRoutines);

			var currentIsAwaiting = false;
			foreach (var routine in awaitingRoutines)
			{
				if (!routine.IsRunning)
				{
					continue;
				}
				else if (routine == Current)
				{
					currentIsAwaiting = true;
				}
				else
				{
					routine.OnException(exception);
				}
			}

			awaitingRoutines.Release();

			if (currentIsAwaiting && Current.IsRunning)
			{
				throw exception;
			}
		}

		/// <summary> Get a routine from the pool. If yield is false routine will resume immediately from await. </summary>
		public static T Get<T>(bool yield) where T : Routine, new()
		{
			var current = Current;
			if (current != null && current.IsDead)
			{
				throw new Exception("Routine is dead!");
			}
			var routine = RoutinePool.GetRoutine<T>();
			routine.Setup(yield, current);
			return routine;
		}


		protected void Start()
		{
			if (_manager == null)
				throw new Exception("Routine is not associated with a manager!");
			if (_state == State.NotStarted)
				MoveNext();
		}

		protected void Finish()
		{
			_state = State.Finished;
			var onFinish = this._onFinish;
			Stop();
			onFinish?.Invoke(); //Resume parent

		}

		internal virtual void Reset()
		{
			Stop();
			_state = State.NotStarted;
			if (_stateMachine != null)
			{
				RoutinePool.StateMachinePool.Release(_stateMachine);
				_stateMachine = null;
			}
			_thrownException = null;
			_parent = null;
			_manager = null;
		}

		protected void OnException(Exception exception)
		{
#if DEBUG
			if (TracingEnabled && !(exception is RoutineException) && !(exception is AggregateException))
			{
				exception = new RoutineException(
					string.Format($"{exception.Message}\n----Async Stack----\n{StackTrace}\n---End Async Stack---\n"),
					exception
				);
			}
#endif

			_thrownException = (_thrownException != null)
				? new AggregateException(_thrownException, exception)
				: exception;

			Finish();
		}

		protected void ThrowPendingException()
		{
			if (_thrownException == null)
				return;

			var exception = _thrownException;
			_thrownException = null;
			throw exception;
		}

		private void Setup(bool yield, Routine parent)
		{
			_id = nextId++;
			SetParent(parent);
			_state = yield ? State.Running : State.NotStarted;
		}

		private void ReleaseChildren()
		{
			foreach (var child in _children)
				child.Release();

			_children.Clear();
		}

		protected void SetParent(Routine newParent)
		{
			if (_parent == newParent)
				return;

			if (_parent != null)
			{
				Assert.IsTrue(_parent._children.Contains(this));
				_parent._children.Remove(this);
			}

			_parent = newParent;

			if (newParent != null)
			{
				_manager = _parent._manager;
				newParent._children.Add(this);
			}
			else
				_manager = null;
		}

		private static void CollectAwaitingRoutines(Routine routine, List<Routine> awaitingRoutines)
		{
			var isAwaiting = true;
			foreach (var child in routine._children)
			{
				if (child.IsRunning)
				{
					isAwaiting = false;
					CollectAwaitingRoutines(child, awaitingRoutines);
				}
			}
			if (isAwaiting)
				awaitingRoutines.Add(routine);
		}
	}
}
