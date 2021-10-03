using System.Collections.Generic;
using System;
using UnityEngine;

namespace SAS.Async
{
	public class AsyncTaskManager
	{
		private List<LightResumer> _nextFrameResumers = new List<LightResumer>();
		private List<LightResumer> _pendingNextFrameResumers = new List<LightResumer>();
		private readonly List<AsyncTask> _roots = new List<AsyncTask>();
		private int _maxRoot = -1;

		/// <summary> 
		/// Resumers managed routines that are waiting for next frame. 
		/// </summary>
		public void Update()
		{
			foreach (var resumer in _nextFrameResumers)
				resumer.Resume();

			_nextFrameResumers.Clear();

			//Cleanup dead routines
			var maxRoot = _maxRoot;
			_maxRoot = -1;
			for (var i = 0; i <= maxRoot; ++i)
			{
				var root = _roots[i];
				if (root == null)
					continue;
				if (root.IsDead)
				{
					_roots[i] = null;
					root.Release();
				}
				else
					_maxRoot = i;
			}
		}

		/// <summary> 
		/// Prepares next-frame resumers. Should be called in LateUpdate. 
		/// </summary>
		public void Flush()
		{
			var temp = _nextFrameResumers;
			_nextFrameResumers = _pendingNextFrameResumers;
			_pendingNextFrameResumers = temp;
		}

		/// <summary> 
		/// Stops all managed routines. 
		/// </summary>
		public void StopAll()
		{
			for (var i = 0; i < _roots.Count; ++i)
			{
				if (_roots[i] == null)
					continue;

				_roots[i].Release();
				_roots[i] = null;
			}
		}

		/// <summary> 
		/// Throws an exception in all managed routines. 
		/// </summary>
		public void ThrowAll(Exception exception)
		{
			for (var i = 0; i < _roots.Count; ++i)
			{
				var root = _roots[i];
				if (root != null)
					root.Throw(exception);
			}
		}

		/// <summary> 
		/// Manages and runs a routine.
		/// </summary>
		public AsyncTaskHandler Run(AsyncTask task, Action<Exception> onStop = null)
		{
			task.SetManager(this, onStop ?? DefaultOnStop);

			var added = false;
			for (var i = 0; i < _roots.Count; ++i)
			{
				if (_roots[i] == null)
				{
					_maxRoot = Mathf.Max(_maxRoot, i);
					_roots[i] = task;
					added = true;
					break;
				}
			}
			if (!added)
			{
				_maxRoot = _roots.Count;
				_roots.Add(task);
			}

			task.MoveNext();
			return new AsyncTaskHandler(task);
		}

		/// <summary>
		/// Internal use only. Schedules a lightweight resumer to be called next frame.
		/// </summary>
		internal void AddNextFrameResumer(ref LightResumer resumer)
		{
			_pendingNextFrameResumers.Add(resumer);
		}

		private static void DefaultOnStop(Exception exception)
		{
			if (exception != null)
			{
				Debug.LogError(exception.ToString());
				if (exception is AggregateException a)
					Debug.LogError(a.Flatten().ToString());
				Debug.LogException(exception);
			}
		}
	}
}
