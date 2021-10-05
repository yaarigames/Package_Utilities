using UnityEngine;
using SAS.TagSystem;

namespace SAS.Utilities
{
    public class MonoBase : MonoBehaviour
    {
        protected virtual void Awake()
        {
            this.Initialize();
        }

        protected virtual void OnDestroy()
        {
            // TODO: Use refection set sett all the properties null;
        }
    }
}
