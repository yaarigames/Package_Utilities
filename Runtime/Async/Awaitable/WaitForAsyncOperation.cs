using UnityEngine;

namespace SAS.Async
{
    public partial class Routine
    {
		/// <summary> 
		/// Routine that yields until an AsyncOperation has completed. 
		/// </summary>
		public static async AsyncTask WaitForAsyncOperation(AsyncOperation asyncOperation)
		{
			if (!asyncOperation.isDone)
			{
				var resumer = RoutinePool.GetResumer<AsyncOperation>();
				asyncOperation.completed += resumer.Resume;
				await resumer;
				resumer.ReleaseResumer();
			}
		}
	}
}
