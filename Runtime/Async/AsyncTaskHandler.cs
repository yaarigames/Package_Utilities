using System;

namespace SAS.Async
{
	public struct AsyncTaskHandler
	{
		private ulong _id;
		private AsyncTask _routine;

		public AsyncTaskHandler(AsyncTask routine)
		{
			_routine = routine;
			_id = routine.Id;
		}

		/// <summary> Indicates if routine is stopped. </summary>
		public bool IsDead { get { return (_routine == null || _id != _routine.Id || _routine.IsDead); } }

		/// <summary> 
		/// Stop the routine.
		/// </summary>
		public void Stop()
		{
			if (_routine != null && _id == _routine.Id)
				_routine.Stop();
		}

		/// <summary> 
		/// Throw an exception in the routine. 
		/// </summary>
		public void Throw(Exception exception)
		{
			if (_routine != null && _id == _routine.Id)
				_routine.Throw(exception);
		}

		public AsyncTask GetAwaiter()
        {
			return _routine;
        }
	}
}
