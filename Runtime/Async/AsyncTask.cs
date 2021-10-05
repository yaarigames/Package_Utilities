using System;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;

namespace SAS.Async
{
	[AsyncMethodBuilder(typeof(AsyncTask))]
	public class AsyncTask : Routine
	{
		public AsyncTask() : base()
		{
			_stepAllAction = StepAll;
			_stepAnyAction = StepAny;
		}

		/// <summary> Assign a manager and stop handler to a routine. </summary>
		public void SetManager(AsyncTaskManager manager, Action<Exception> onStop)
		{
			SetParent(null);
			_manager = manager;
			_onStop = onStop;
		}

		/// <summary> Internal use only. Required for awaiter. </summary>
		public void GetResult()
		{
			ThrowPendingException();
		}

		/// <summary> Internal use only. Required for task-like. </summary>
		public AsyncTask GetAwaiter()
		{
			Start(); //Step when passed to await
			return this;
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			var stateMachineRef = RoutinePool.StateMachinePool.Get<StateMachineRef<TStateMachine>>();
			stateMachineRef.value = stateMachine;
			this._stateMachine = stateMachineRef;
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public void SetStateMachine(IAsyncStateMachine stateMachine) { }

		/// <summary> Internal use only. Required for task builder. </summary>
		public void SetResult()
		{
			Assert.IsTrue(_state != State.Finished);
			_state = State.Finished;
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public void SetException(Exception exception)
		{
			OnException(exception);
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
			where TAwaiter : INotifyCompletion
			where TStateMachine : IAsyncStateMachine
		{
			awaiter.OnCompleted(_stepAction);
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
			where TAwaiter : ICriticalNotifyCompletion
			where TStateMachine : IAsyncStateMachine
		{
			awaiter.OnCompleted(_stepAction);
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public AsyncTask Task { get { return this; } }

		/// <summary> Internal use only. Steps a routine only if all of it's children are finished. </summary>
		public void StepAll()
		{
			foreach (var child in _children)
			{
				if (!child.IsCompleted)
					return;
			}

			//Propagate exceptions
			foreach (var child in _children)
			{
				var childException = (child as AsyncTask)._thrownException;
				if (childException != null)
				{
					_thrownException = (_thrownException != null)
						? new AggregateException(_thrownException, childException)
						: childException;
				}
			}
			SetResult(); //Mark as finished

			MoveNext();
		}

		/// <summary> Internal use only. Steps a routine and sets it's result from the first completed child. </summary>
		public void StepAny()
		{
			//Propagate exception
			foreach (var child in _children)
			{
				if (child.IsCompleted)
				{
					var childException = (child as AsyncTask)._thrownException;
					_thrownException = childException;
					break;
				}
			}
			SetResult(); //Mark as finished

			MoveNext();
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public static AsyncTask Create()
		{
			var routine = Get<AsyncTask>(false);
			routine.Trace(2);
			return routine;
		}
	}

	[AsyncMethodBuilder(typeof(AsyncTask<>))]
	public class AsyncTask<T> : Routine
	{
		private T _result = default(T);

		public AsyncTask() : base()
		{
			_stepAnyAction = StepAny;
		}

		/// <summary> Internal use only. Required for awaiter. </summary>
		public T GetResult()
		{
			ThrowPendingException();
			return _result;
		}

		/// <summary> Internal use only. Required for task-like. </summary>
		public AsyncTask<T> GetAwaiter()
		{
			Start(); //Step when passed to await
			return this;
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			var stateMachineRef = RoutinePool.StateMachinePool.Get<StateMachineRef<TStateMachine>>();
			stateMachineRef.value = stateMachine;
			_stateMachine = stateMachineRef;
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public void SetStateMachine(IAsyncStateMachine stateMachine) { }

		/// <summary> Internal use only. Required for task builder. </summary>
		public void SetResult(T result)
		{
			_result = result;
			Assert.IsTrue(_state != State.Finished);
			_state = State.Finished;
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public void SetException(Exception exception)
		{
			OnException(exception);
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
			where TAwaiter : INotifyCompletion
			where TStateMachine : IAsyncStateMachine
		{
			awaiter.OnCompleted(_stepAction);
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
			where TAwaiter : ICriticalNotifyCompletion
			where TStateMachine : IAsyncStateMachine
		{
			awaiter.OnCompleted(_stepAction);
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public AsyncTask<T> Task { get { return this; } }

		/// <summary>
		/// Internal use only. Steps a routine only if all of it's children are finished and sets the result array.
		/// </summary>
		public void MoveNextAll<I>()
		{
			foreach (var child in _children)
			{
				if (!child.IsCompleted)
					return;
			}

			//Build results array and propagate exceptions
			var resultArray = new I[_children.Count];
			for (var i = 0; i < _children.Count; ++i)
			{
				var child = (_children[i] as AsyncTask<I>);
				resultArray[i] = child._result;

				var childException = child._thrownException;
				if (childException != null)
				{
					_thrownException = (_thrownException != null)
						? new AggregateException(_thrownException, childException)
						: childException;
				}
			}
			(this as AsyncTask<I[]>).SetResult(resultArray);

			MoveNext();
		}

		/// <summary> Internal use only. Steps a routine and sets it's result from the first completed child. </summary>
		public void StepAny()
		{
			foreach (var child in _children)
			{
				//Propagate result and exception
				if (child.IsCompleted)
				{
					var _child = (child as AsyncTask<T>);
					_thrownException = _child._thrownException;
					SetResult(_child._result);
					break;
				}
			}

			MoveNext();
		}

		/// <summary> Internal use only. Required for task builder. </summary>
		public static AsyncTask<T> Create()
		{
			var routine = Get<AsyncTask<T>>(false);
			routine.Trace(2);
			return routine;
		}

		internal override void Reset()
		{
			_result = default(T);
			base.Reset();
		}
	}
}