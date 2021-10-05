using System;

namespace SAS.Async
{
	internal class Resumer : IResumer
	{
		public AsyncTask routine = null;
		public ulong id = 0;
		public bool WasResumed { get; private set; }

		public void Resume()
		{
			if (routine != null)
			{
				if (id == routine.Id)
				{
					var _task = routine;
					Reset();
					_task.MoveNext();
				}
			}
			else
			{
				WasResumed = true;
			}
		}

		public void Reset()
		{
			WasResumed = false;
			routine = null;
			id = 0;
		}
	}

	internal class Resumer<T> : IResumer<T>
	{
		public AsyncTask<T> routine = null;
		public ulong id = 0;
		public T result = default(T);
		public bool WasResumed { get; private set; }

		public void Resume(T result)
		{
			if (routine != null)
			{
				if (id == routine.Id)
				{
					var _task = routine;
					Reset();
					_task.SetResult(result);
					_task.MoveNext();
				}
			}
			else
			{
				this.result = result;
				WasResumed = true;
			}
		}

		public void Reset()
		{
			WasResumed = false;
			routine = null;
			id = 0;
		}
	}

	public struct LightResumer : IResumer
	{
		public AsyncTask routine;
		public ulong id;

		public void Resume()
		{
			if (id == routine.Id)
				routine.MoveNext();
		}

		public void Reset()
		{
			routine = null;
			id = 0;
		}
	}
}
