using System.Collections.Generic;
using UnityEngine.Assertions;

namespace SAS.Async
{
    public partial class Routine
    {
		/// <summary> 
		/// Routine that yields until all routines in a collection complete. 
		/// </summary>
		public static AsyncTask WhenAll(IEnumerable<Routine> routines)
		{
			var allRoutine = Get<AsyncTask>(true);
			allRoutine.Trace(1);
			var isCompleted = true;
			var currentId = allRoutine._id;
			foreach (var routine in routines)
			{
				routine.SetParent(allRoutine);
				routine.Start();
				if (allRoutine._id != currentId)
				{
					Assert.IsTrue(allRoutine.IsDead);
					return allRoutine;
				}
				if (!routine.IsCompleted)
				{
					routine.OnCompleted(allRoutine._stepAllAction);
					isCompleted = false;
				}
			}
			if (isCompleted)
				allRoutine.StepAll();

			return allRoutine;
		}

		/// <summary> 
		/// Routine that yields until all routines in a collection complete. 
		/// </summary>
		public static AsyncTask WhenAll(params Routine[] routines)
		{
			var allRoutine = WhenAll((IEnumerable<Routine>)routines);
			allRoutine.Trace(1);
			return allRoutine;
		}

		/// <summary>
		/// Routine that yields until all routines in a collection complete. Returns array of results.
		/// </summary>
		public static AsyncTask<T[]> WhenAll<T>(IEnumerable<AsyncTask<T>> routines)
		{
			var allRoutine = Get<AsyncTask<T[]>>(true);
			if (allRoutine._stepAllAction == null)
				allRoutine._stepAllAction = allRoutine.MoveNextAll<T>;
			allRoutine.Trace(1);
			var isCompleted = true;
			var currentId = allRoutine._id;
			foreach (var routine in routines)
			{
				routine.SetParent(allRoutine);
				routine.Start();
				if (allRoutine._id != currentId)
				{
					Assert.IsTrue(allRoutine.IsDead);
					return allRoutine;
				}
				if (!routine.IsCompleted)
				{
					routine.OnCompleted(allRoutine._stepAllAction);
					isCompleted = false;
				}
			}
			if (isCompleted)
				allRoutine.MoveNextAll<T>();

			return allRoutine;
		}

		/// <summary>
		/// Routine that yields until all routines in a collection complete. Returns array of results.
		/// </summary>
		public static AsyncTask<T[]> WhenAll<T>(params AsyncTask<T>[] routines)
		{
			var allRoutine = WhenAll((IEnumerable<AsyncTask<T>>)routines);
			allRoutine.Trace(1);
			return allRoutine;
		}
	}
}
