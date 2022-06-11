using System;
using UnityEngine;

namespace SAS.Utilities.TagSystem
{
    public class BaseContext : MonoBase, IContext
    {
        [Tooltip("If True, this GameObject will be marked as DontDestroyOnLoad. Make Sure Only one context is there for which  isCrossContextBinder is true")]
        [SerializeField] private bool m_IsCrossContextBinder;
        [SerializeField] public Binder m_Binder;

        protected override void Awake()
        {
            if (m_IsCrossContextBinder)
            {
                if (!ComponentExtensions._cachedContext.TryGetValue("DontDestroyOnLoad", out var context))
                {
                    DontDestroyOnLoad(gameObject);
                    ComponentExtensions._cachedContext.Add("DontDestroyOnLoad", this);
                }
                else
                    Debug.LogWarning($"There is already an CrossContextBinder wit the name {context.GetType().Name} ");
            }
            base.Awake();
        }

        object IContext.GetOrCreate(Type type, string tag)
        {
            return m_Binder.GetOrCreate(type, tag);
        }


        bool IContext.TryGet(Type type, out object instance, string tag)
        {
            return m_Binder.TryGet(type, out instance, tag);
        }
    }
}
