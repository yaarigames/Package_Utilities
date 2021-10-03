using System;
using UnityEngine;

namespace SAS.Async
{
	public class RoutineManagerBehaviour : MonoBehaviour
	{
		public AsyncTaskManager Manager { get; } = new AsyncTaskManager();

		protected virtual void Update() => Manager.Update();
		protected virtual void LateUpdate() => Manager.Flush();
		protected virtual void OnDestroy() => StopAll();

		/// <summary> 
		/// Manages and runs a routine. 
		/// </summary>
		public AsyncTaskHandler Run(AsyncTask routine, Action<Exception> onStop = null)
		{
			return Manager.Run(routine, onStop);
		}

		/// <summary> Stops all managed routines. </summary>
		public void StopAll()
		{
			Manager.StopAll();
		}

		/// <summary> Throws an exception in all managed routines. </summary>
		public void ThrowAll(Exception exception)
		{
			Manager.ThrowAll(exception);
		}
	}
}
