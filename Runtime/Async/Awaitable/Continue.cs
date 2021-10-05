namespace SAS.Async
{
    public partial class Routine
    {
		/// <summary>
		/// Do-nothing routine that resumes immediately. Good for quieting warning about async functions with no await
		/// statement.
		/// </summary>
		public static AsyncTask Continue()
		{
			var continueRoutine = Get<AsyncTask>(false);
			continueRoutine.SetResult();
			return continueRoutine;
		}

		/// <summary>
		/// Do-nothing routine that resumes immediately with the specified result. Good for quieting warning about async
		/// functions with no await statement.
		/// </summary>
		public static AsyncTask<T> Continue<T>(T result)
		{
			var continueRoutine = Get<AsyncTask<T>>(false);
			continueRoutine.SetResult(result);
			return continueRoutine;
		}
	}
}
