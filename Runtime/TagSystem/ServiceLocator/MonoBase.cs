using UnityEngine;

namespace SAS.Utilities.TagSystem
{
    public class MonoBase : MonoBehaviour
    {
        [SerializeField] private bool m_InitializeOnStart = false;

        protected virtual void Awake()
        {
            if (!m_InitializeOnStart)
                this.Initialize();
        }

        protected virtual void Start()
        {
            if (m_InitializeOnStart)
                this.Initialize();
        }

        protected virtual void OnDestroy()
        {
            // TODO: Use refection set sett all the properties null;
        }
    }
}
