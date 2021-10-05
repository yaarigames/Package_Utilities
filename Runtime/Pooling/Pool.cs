using System.Collections.Generic;
using System;

namespace SAS.Pool
{
	public class Pool<T> where T : class, new()
	{
		private readonly Stack<T> _pool = new Stack<T>();
		private int _activeCount = 0;

		public T Get()
		{
			++_activeCount;
			return (_pool.Count > 0) ? (_pool.Pop() as T) : new T();
		}

		public void Release(T obj)
		{
			--_activeCount;
			_pool.Push(obj);
		}

		public void Clear()
		{
			_pool.Clear();
		}

		public string Report() { return string.Format("{0}/{1}", _activeCount, _pool.Count); }
	}

	public class TypedPool<I>
	{
		private readonly Dictionary<Type, Stack<I>> _pools = new Dictionary<Type, Stack<I>>();
		private int _activeCount = 0;

		public T Get<T>() where T : class, I, new()
		{
			++_activeCount;
			var pool = GetPool(typeof(T));
			return (pool.Count > 0) ? (pool.Pop() as T) : new T();
		}

		public void Release(I obj)
		{
			--_activeCount;
			var pool = GetPool(obj.GetType());
			pool.Push(obj);
		}

		public void Clear()
		{
			_pools.Clear();
		}

		public string Report()
		{
			var c = 0;
			foreach (var t in _pools.Values)
			{
				c += t.Count;
			}
			return string.Format("{0}/{1}", _activeCount, c);
		}

		private Stack<I> GetPool(Type type)
		{
			if (!_pools.ContainsKey(type))
			{
				var pool = new Stack<I>();
				_pools[type] = pool;
				return pool;
			}
			return _pools[type];
		}
	}
}
