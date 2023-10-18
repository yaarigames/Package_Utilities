using SAS.Utilities.TagSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SAS.Pool
{
    public abstract class PoolSO<T> : ScriptableObject, IPool<T>
    {
        protected readonly Stack<T> Available = new Stack<T>();
        protected abstract IFactory<T> Factory { get; }

       
        public int Inactive => Available.Count;
        public int Capacity => _active + Inactive;
        public int Active => _active;

        protected virtual bool Create(out T item)
        {
            return Factory.Create(out item);
        }

        [NonSerialized] protected bool _prewarmed = false;
        [NonSerialized] private int _active;

        public void Initialize(int count)
        {
            if (_prewarmed)
            {
                Debug.LogWarning($"Pool {name} has already been prewarmed.");
                return;
            }
            
            ExpandBy(count);
            _prewarmed = true;
        }

        private void ExpandBy(int expansionCount)
        {
            for (int i = 0; i < expansionCount; i++)
            {
                if (Create(out var item))
                    Available.Push(item);
                else
                {
                    Debug.LogWarning($"Pool {name} can't create new item!");
                    break;
                }
            }
        }

        public virtual T Spawn<O>(O data, MonoBase parent = null)
        {
            if (Available.Count == 0)
                ExpandBy(4);
            try
            {
                var item = Available.Pop();
                _active++;
                return item;
            }
            catch
            {
                return default;
            }
        }

        public T Spawn(MonoBase parent = null)
        {
           return Spawn<object>(default,parent);
        }

        public virtual void Despawn(T item)
        {
            --_active;
            Available.Push(item);
        }

        public void Clear()
        {
            Available.Clear();
        }
    }
}
