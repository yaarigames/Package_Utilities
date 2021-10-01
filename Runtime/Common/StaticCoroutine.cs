using System.Collections;
using UnityEngine;

namespace SAS.Utilities
{
    public class StaticCoroutine : AutoInstantiateSingleton<StaticCoroutine>
    {
        protected override void Awake()
        {
            _PersistentOnSceneChange = true;
            base.Awake();
        }

        public static Coroutine Start(IEnumerator coroutine)
        {
           return Instance.StartCoroutine(coroutine);
        }
    }
}
