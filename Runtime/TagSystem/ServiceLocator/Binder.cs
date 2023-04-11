using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SAS.Utilities.TagSystem
{
    public interface IBindable
    {
        void OnInstanceCreated();
    }

    [Serializable, CreateAssetMenu(menuName = "SAS/Binder")]
    public class Binder : ScriptableObject
    {
        [Serializable]
        public class Binding
        {
            [SerializeField] private string m_Interface;
            [SerializeField] private string m_Type;
            [SerializeField] private string m_Tag;
            public Type InterfaceType => Type.GetType(m_Interface);
            public string Tag => m_Tag;
            public object CreateInstance()
            {
                object instance = default;
                Type type = Type.GetType(m_Type);

                if (type.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    var result = GameObject.FindObjectOfType(type);
                    if (result != null)
                    {
                        instance = ((Component)result).GetComponent(type, Tag);
                        if (instance == null)
                            Debug.LogError($"No GameObject having component attached of the type:  {m_Type} with  tag: {m_Tag} found");
                    }
                    else
                        Debug.LogError($"No GameObject having component attached of the type:  {m_Type} with  tag: {m_Tag} found");
                }
                else
                {
                    instance = Activator.CreateInstance(Type.GetType(m_Type));
                   
                }

                InvokeInjecttionEvent((IBindable)instance);
                return instance;
            }

            private void InvokeInjecttionEvent(IBindable bindable)
            {
                bindable.OnInstanceCreated();
            }
        }

       

        [SerializeField] private Binding[] m_Bindings;
        private Dictionary<string, object> _cachedBindings = new Dictionary<string, object>();

        private string GetKey(Type type, string tag)
        {
            return $"{type.Name}{tag}";
        }

        public T GetOrCreate<T>(string tag = "")
        {
            return (T)GetOrCreate(typeof(T), tag);
        }

        public object GetOrCreate(Type type, string tag = "")
        {
            var key = GetKey(type, tag);
            if (!_cachedBindings.TryGetValue(key, out var value))
            {
                var instance = CreateInstance(type, tag);
                Add(type, instance, tag);
                return instance;
            }

            return value;
        }

        public bool TryGet(Type type, out object instance, string tag = "")
        {
            var key = GetKey(type, tag);
            if (!_cachedBindings.TryGetValue(key, out instance))
            {
                instance = null;
                Debug.LogError($"Required service of type {type.Name} with tag {tag} is not found");
                return false;
            }

            return true;
        }

        private void Add(Type type, object instance, string tag = "")
        {
            var key = GetKey(type, tag);
           if (!_cachedBindings.TryGetValue(key,  out object cachedInstance))
                _cachedBindings.Add(key, instance);

            var baseTypes = type.GetInterfaces();
            if (type.BaseType != null)
                baseTypes = baseTypes.Prepend(type.BaseType).ToArray();

            foreach (var baseType in baseTypes)
                Add(baseType, instance, tag);
        }

        private object CreateInstance(Type type, string tag)
        {
            var binding = Array.Find(m_Bindings, ele => ele.InterfaceType.Equals(type) && ele.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase));
            return binding?.CreateInstance();
        }
    }
}
