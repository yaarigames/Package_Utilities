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
    }
}
