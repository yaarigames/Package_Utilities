using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SAS.TagSystem
{
    public static class ComponentExtensions
    {
        private static Dictionary<Type, Func<Component, Type, bool, Component>> _componentFetchers = new Dictionary<Type, Func<Component, Type, bool, Component>>
        {
            { typeof(FieldRequiresSelfAttribute), (comp, type, includeInactive) => comp.GetComponent(type) },
            { typeof(FieldRequiresChildAttribute), (comp, type, includeInactive) => comp.GetComponentInChildren(type, includeInactive) },
            { typeof(FieldRequiresParentAttribute), (comp, type, includeInactive) => comp.GetComponentInParent(type) },
        };

        private static Dictionary<Type, Func<Component, Type, string, bool, Component>> _componentWithTagFetchers = new Dictionary<Type, Func<Component, Type, string, bool, Component>>
        {
            { typeof(FieldRequiresSelfAttribute), (comp, type, tag, includeInactive) => comp.GetComponent(type, tag) },
            { typeof(FieldRequiresChildAttribute), (comp, type, tag, includeInactive) => comp.GetComponentInChildren(type, tag, includeInactive) },
            { typeof(FieldRequiresParentAttribute), (comp, type, tag, includeInactive) => comp.GetComponentInParent(type, tag, includeInactive) },
        };

        private static Dictionary<Type, Func<Component, Type, bool, Component[]>> _componentsFetchers
           = new Dictionary<Type, Func<Component, Type, bool, Component[]>>
           {
                { typeof(FieldRequiresSelfAttribute), (comp, type, includeInactive) => comp.GetComponents(type) },
                { typeof(FieldRequiresChildAttribute), (comp, type, includeInactive) => comp.GetComponentsInChildren(type, includeInactive) },
                { typeof(FieldRequiresParentAttribute), (comp, type, includeInactive) => comp.GetComponentsInParent(type, includeInactive) },
           };

        private static Dictionary<Type, Func<Component, Type, string, bool, Component[]>> _componentsWithTagFetchers
            = new Dictionary<Type, Func<Component, Type, string, bool, Component[]>>
            {
                { typeof(FieldRequiresSelfAttribute), (comp, type, tag, includeInactive) => comp.GetComponents(type, tag) },
                { typeof(FieldRequiresChildAttribute), (comp, type, tag, includeInactive) => comp.GetComponentsInChildren(type, tag, includeInactive) },
                { typeof(FieldRequiresParentAttribute), (comp, type, tag, includeInactive) => comp.GetComponentsInParent(type, tag, includeInactive) },
            };

        public static void Initialize(this Component component)
        {
            var allFields = GetAllFields(component);
            foreach (var field in allFields)
            {
                var requirement = field.GetCustomAttribute<BaseRequiresComponent>(false);
                if (requirement != null)
                {
                    if (field.FieldType.IsArray)
                    {
                        var elementType = field.FieldType.GetElementType();
                         var dependencies = default(Component[]);
                        if (string.IsNullOrEmpty(requirement.tag))
                            dependencies = _componentsFetchers[requirement.GetType()](component, elementType, requirement.includeInactive);
                        else
                             dependencies = _componentsWithTagFetchers[requirement.GetType()](component, elementType, requirement.tag, requirement.includeInactive);

                        field.SetValue(component, ConvertArray(dependencies, elementType));
                    }
                    else
                    {
                        var dependency = default(Component);
                        if (string.IsNullOrEmpty(requirement.tag))
                            dependency = _componentFetchers[requirement.GetType()](component, field.FieldType, requirement.includeInactive);
                        else
                            dependency = _componentWithTagFetchers[requirement.GetType()](component, field.FieldType, requirement.tag, requirement.includeInactive);
                        field.SetValue(component, dependency);
                    }
                }
            }
        }

        private static IEnumerable<FieldInfo> GetAllFields(this Component component)
        {
            var componentType = component.GetType();
            var baseType = componentType.BaseType;

            FieldInfo[] fields = componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var enumerable = fields.AsEnumerable();

            while (baseType != null)
            {
                var baseFields = baseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                enumerable = enumerable.Concat(baseFields);
                baseType = baseType.BaseType;
            }

            return fields.AsEnumerable();
        }

        private static Array ConvertArray<T>(T[] elements, Type castType)
        {
            var array = Array.CreateInstance(castType, elements.Length);
            for (var i = 0; i < elements.Length; ++i)
                array.SetValue(elements[i], i);

            return array;
        }
    }
}
