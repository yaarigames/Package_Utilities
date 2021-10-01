using UnityEngine;

namespace SAS.Pool
{
    public abstract class Poolable : MonoBehaviour
    {
        internal SpawnablePoolSO ObjectPool { get; set; }
        private ISpawnable[] _spawnables;
        protected virtual void Awake()
        {
            _spawnables = GetComponentsInChildren<ISpawnable>();
        }

        public void Destroy()
        {
            if (ObjectPool != null)
                ObjectPool.Despawn(this);
        }

        internal void OnSpawn()
        {
            foreach (var spawnable in _spawnables)
                spawnable.OnSpawn();
        }

        internal void OnDespawn()
        {
            foreach (var spawnable in _spawnables)
                spawnable.OnDespawn();
        }
    }
}
