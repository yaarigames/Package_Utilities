using System;
using UnityEngine;

namespace SAS.TagSystem
{
    public class BaseContext : MonoBehaviour, IContext
    {

        [SerializeField] public Binder m_Binder;
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
