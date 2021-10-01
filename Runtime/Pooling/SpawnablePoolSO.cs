using UnityEngine;

namespace SAS.Pool
{
    [CreateAssetMenu(menuName = "SAS/Pool/Spawnable Object")]
    public class SpawnablePoolSO : ComponentPoolSO<Poolable>
    {
        [SerializeField] private SpawnableFactorySO _factory = default;
        protected override IFactory<Poolable> Factory => _factory;

        protected override bool Create(out Poolable item)
        {
            if (base.Create(out item))
                item.ObjectPool = this;
            return item != null;
        }

        public override Poolable Spawn()
        {
            var item = base.Spawn();
            item?.OnSpawn();
            return item;
        }

        public override void Despawn(Poolable item)
        {
            base.Despawn(item);
            item.OnDespawn();
        }
    }
}

