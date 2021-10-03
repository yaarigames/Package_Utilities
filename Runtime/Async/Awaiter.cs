using UnityEngine;
using UnityEngine.Assertions;

namespace SAS.Async
{
	public static class Awaiter
	{
		public static AsyncTask GetAwaiter(this AsyncOperation asyncOperation)
		{
			return AsyncTask.WaitForAsyncOperation(asyncOperation).GetAwaiter();
		}

		public static AsyncTask GetAwaiter(this CustomYieldInstruction customYieldInstruction)
		{
			return AsyncTask.WaitForCustomYieldInstruction(customYieldInstruction).GetAwaiter();
		}

		public static AsyncTask GetAwaiter(this IResumer resumer)
		{
			var _resumer = resumer as Resumer;
			Assert.IsNotNull(_resumer);
			var resumerRoutine = AsyncTask.Get<AsyncTask>(true);
			resumerRoutine.Trace(1);
			_resumer.routine = resumerRoutine;
			_resumer.id = resumerRoutine.Id;
			if (_resumer.WasResumed)
			{
				resumerRoutine.SetResult();
				_resumer.Reset();
			}
			return resumerRoutine;
		}

		public static AsyncTask<T> GetAwaiter<T>(this IResumer<T> resumer)
		{
			var _resumer = resumer as Resumer<T>;
			Assert.IsNotNull(_resumer);
			var resumerRoutine = AsyncTask.Get<AsyncTask<T>>(true);
			resumerRoutine.Trace(1);
			_resumer.routine = resumerRoutine;
			_resumer.id = resumerRoutine.Id;
			if (_resumer.WasResumed)
			{
				resumerRoutine.SetResult(_resumer.result);
				_resumer.Reset();
			}
			return resumerRoutine;
		}
	}
}