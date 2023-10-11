using SAS.Utilities.TagSystem;
using UnityEngine;

namespace SAS.Pool
{
    public abstract class ComponentPoolSO<T> : PoolSO<T> where T : Component
    {
        private Transform _poolRoot;
        private Transform _parent;

        private Transform PoolRoot
        {
            get
            {
                if (_poolRoot == null)
                {
                    _poolRoot = new GameObject(name).transform;
                    _poolRoot.SetParent(_parent);
                }
                return _poolRoot;
            }
        }

        public void SetParent(Transform t)
        {
            _parent = t;
            PoolRoot.SetParent(_parent);
        }

        protected override bool Create(out T item)
        {
            if (base.Create(out item))
            {
                item.transform.SetParent(PoolRoot.transform);
                item.gameObject.SetActive(false);
                return true;
            }

            return false;
        }

        public override T Spawn<O>(O data, MonoBase parent = null)
        {
            T item = base.Spawn(data, parent);
            item.gameObject.SetActive(true);
            return item;
        }

        public override void Despawn(T item)
        {
            item.transform.SetParent(PoolRoot.transform, false);
            item.gameObject.SetActive(false);
            base.Despawn(item);
        }
    }
}
