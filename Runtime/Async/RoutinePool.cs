using SAS.Pool;
using System.Collections.Generic;
using UnityEngine;
using static SAS.Async.Routine;

namespace SAS.Async
{
	public static class RoutinePool
	{
		internal static readonly TypedPool<IStateMachineRef> StateMachinePool = new TypedPool<IStateMachineRef>();
		private static readonly TypedPool<Routine> pool = new TypedPool<Routine>();
		private static readonly TypedPool<IResumerBase> resumerPool = new TypedPool<IResumerBase>();
		public static readonly Pool<List<Routine>> awaiterListPool = new Pool<List<Routine>>();

		public static List<Routine> GetAwaiterList()
		{
			return awaiterListPool.Get();
		}

		public static T GetRoutine<T>() where T : Routine, new()
		{
			return pool.Get<T>();
		}


		/// <summary> Get a resumer from the pool. </summary>
		/*public static IResumer GetResumer()
		{
			return resumerPool.Get<Resumer>();
		}

		/// <summary> Get a resumer from the pool. </summary>
		public static IResumer<T> GetResumer<T>()
		{
			return resumerPool.Get<Resumer<T>>();
		}

		

		/// <summary> Release a resumer to the pool. </summary>
		public static void Release(IResumer resumer)
		{
			resumer.Reset();
			resumerPool.Release(resumer);
		}

		/// <summary> Release a resumer to the pool. </summary>
		public static void Release<T>(IResumer<T> resumer)
		{
			resumer.Reset();
			resumerPool.Release(resumer);
		}

		/// <summary> Release routine back to pool. </summary>
		public static void Release(RoutineBase routine)
		{
			//routine.Reset();
			pool.Release(routine);
		}

		internal static void Release(ref IStateMachineRef stateMachine)
		{
			if (stateMachine == null)
				return;
			stateMachinePool.Release(stateMachine);
			stateMachine = null;
		}
	*/

		/// <summary> Get a resumer from the pool. </summary>
		public static IResumer GetResumer()
		{
			return resumerPool.Get<Resumer>();
		}

		/// <summary> Get a resumer from the pool. </summary>
		public static IResumer<T> GetResumer<T>()
		{
			return resumerPool.Get<Resumer<T>>();
		}

		/// <summary> Release a resumer to the pool. </summary>
		public static void ReleaseResumer(this IResumer resumer)
		{
			resumer.Reset();
			resumerPool.Release(resumer);
		}

		/// <summary> Release a resumer to the pool. </summary>
		public static void ReleaseResumer<T>(this IResumer<T> resumer)
		{
			resumer.Reset();
			resumerPool.Release(resumer);
		}

		public static void Release(this List<Routine> awaitingRoutines)
		{
			awaiterListPool.Release(awaitingRoutines);
			awaitingRoutines.Clear();
		}

		/// <summary> Release routine back to pool. </summary>
		public static void Release(this Routine routine)
		{
			routine.Reset();
			pool.Release(routine);
		}

		/// <summary> Dump pooled objects to clear memory. </summary>
		public static void ClearPools()
		{
			StateMachinePool.Clear();
			pool.Clear();
			resumerPool.Clear();
			awaiterListPool.Clear();
		}

		public static void Report()
		{
			Debug.LogFormat("stateMachinePool = {0}", StateMachinePool.Report());
			Debug.LogFormat("pool = {0}", pool.Report());
			Debug.LogFormat("resumerPool = {0}", resumerPool.Report());
			Debug.LogFormat("awaiterListPool = {0}", awaiterListPool.Report());
		}
	}
}
