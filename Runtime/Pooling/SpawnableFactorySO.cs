using UnityEngine;

namespace SAS.Pool
{
    [CreateAssetMenu(menuName = "SAS/Factory/Spawnable Object")]
    public class SpawnableFactorySO : FactorySO<Poolable>
    {
        [SerializeField] private GameObject m_Prefab = default;

        public override bool Create(out Poolable item)
        {
            item = Instantiate(m_Prefab).GetComponent<Poolable>();
            return item != null;
        }
    }
}
