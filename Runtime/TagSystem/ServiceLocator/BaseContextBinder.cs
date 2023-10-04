using System;
using UnityEngine;

namespace SAS.Utilities.TagSystem
{
    public class BaseContextBinder : MonoBase, IContextBinder
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

        object IContextBinder.GetOrCreate(Type type, Tag tag)
        {
            return m_Binder.GetOrCreate(type, tag);
        }


        bool IContextBinder.TryGet(Type type, out object instance, Tag tag)
        {
            return m_Binder.TryGet(type, out instance, tag);
        }

        bool IContextBinder.TryGet<T>(out T instance, Tag tag)
        {
            instance = default;

            if ((this as IContextBinder).TryGet(typeof(T), out object result, tag))
            {
                instance = (T)result;
                return true;
            }

            return false;
        }

        void IContextBinder.Add(Type type, object instance, Tag tag)
        {
            m_Binder.Add(type, instance, tag);
        }
    }
}
