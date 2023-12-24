using UnityEngine;

namespace SAS.Pool
{
    internal sealed class PoolClearer : MonoBehaviour
    {
        private IPool _pool;

        private void OnApplicationQuit()
        {
            _pool?.Clear();
        }

        internal void Init(IPool pool)
        {
            _pool = pool;
        }

        void OnDestroy()
        {
            _pool.Clear();
        }
    }
}
