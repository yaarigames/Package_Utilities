using SAS.Utilities.TagSystem;
using UnityEngine;

namespace SAS.Pool
{
    [CreateAssetMenu(menuName = "SAS/Pool/Spawnable Object")]
    public class SpawnablePoolSO : ComponentPoolSO<Poolable>
    {
        [SerializeField] private FactorySO<Poolable> _factory = default;
        protected override IFactory<Poolable> Factory => _factory;

        protected override bool Create(out Poolable item)
        {
            if (base.Create(out item))
                item.ObjectPool = this;
            return item != null;
        }

        public override Poolable Spawn<O>(O obj, MonoBase parent = null)
        {
            var item = base.Spawn(obj, parent);
            item?.OnSpawn(obj);
            return item;
        }

        public override void Despawn(Poolable item)
        {
            base.Despawn(item);
            item.OnDespawn();
        }
    }
}

