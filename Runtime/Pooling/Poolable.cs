using SAS.Utilities.TagSystem;

namespace SAS.Pool
{
    public abstract class Poolable : MonoBase
    {
        internal ComponentPoolSO<Poolable> ObjectPool { get; set; }
        private ISpawnable[] _spawnables;
        public bool active { get; private set; } = false;
        protected override void Awake()
        {
            _spawnables = GetComponentsInChildren<ISpawnable>();
        }

        public void Despawn()
        {
            if (ObjectPool != null)
                ObjectPool.Despawn(this);
        }

        internal void OnSpawn<T>(T obj)
        {
            active = true;
            foreach (var spawnable in _spawnables)
                spawnable.OnSpawn(obj);
        }

        internal void OnDespawn()
        {
            active = false;
            foreach (var spawnable in _spawnables)
                spawnable.OnDespawn();
        }
    }
}
