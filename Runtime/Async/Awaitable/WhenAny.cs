using System.Collections.Generic;
using UnityEngine.Assertions;

namespace SAS.Async
{
    public partial class Routine
    {
		/// <summary>
		/// Routine that yields until the first routine in a collection completes. The others will be stopped at that
		/// time.
		/// </summary>
		public static AsyncTask WhenAny(IEnumerable<Routine> routines)
		{
			var anyRoutine = Get<AsyncTask>(true);
			anyRoutine.Trace(1);
			anyRoutine._stopChildrenOnStep = true;
			var isCompleted = false;
			var currentId = anyRoutine._id;
			foreach (var routine in routines)
			{
				routine.SetParent(anyRoutine);
				if (!isCompleted)
				{
					routine.Start();
					if (anyRoutine._id != currentId)
					{
						Assert.IsTrue(anyRoutine.IsDead);
						return anyRoutine;
					}
					isCompleted = routine.IsCompleted;

					if (!isCompleted)
						routine.OnCompleted(anyRoutine._stepAnyAction);
				}
			}

			if (anyRoutine._children.Count == 0 || isCompleted)
				anyRoutine.StepAny();

			return anyRoutine;
		}

		/// <summary>
		/// Routine that yields until the first routine in a collection completes. The others will be stopped at that
		/// time.
		/// </summary>
		public static AsyncTask WhenAny(params Routine[] routines)
		{
			var anyRoutines = WhenAny((IEnumerable<Routine>)routines);
			anyRoutines.Trace(1);
			return anyRoutines;
		}

		/// <summary>
		/// Routine that yields until the first routine in a collection completes. The others will be stopped at that
		/// time. Returns result from completed routine.
		/// </summary>
		public static AsyncTask<T> WhenAny<T>(IEnumerable<AsyncTask<T>> routines)
		{
			var anyRoutine = Get<AsyncTask<T>>(true);
			anyRoutine.Trace(1);
			anyRoutine._stopChildrenOnStep = true;
			var isCompleted = false;
			var currentId = anyRoutine._id;
			foreach (var routine in routines)
			{
				routine.SetParent(anyRoutine);
				if (!isCompleted)
				{
					routine.Start();
					if (anyRoutine._id != currentId)
					{
						Assert.IsTrue(anyRoutine.IsDead);
						return anyRoutine;
					}
					isCompleted = routine.IsCompleted;

					if (!isCompleted)
						routine.OnCompleted(anyRoutine._stepAnyAction);
				}
			}

			if (anyRoutine._children.Count == 0 || isCompleted)
				anyRoutine.StepAny();

			return anyRoutine;
		}

		/// <summary>
		/// Routine that yields until the first routine in a collection completes. The others will be stopped at that
		/// time. Returns result from completed routine.
		/// </summary>
		public static AsyncTask<T> WhenAny<T>(params AsyncTask<T>[] routines)
		{
			var anyRoutine = WhenAny((IEnumerable<AsyncTask<T>>)routines);
			anyRoutine.Trace(1);
			return anyRoutine;
		}
	}
}
