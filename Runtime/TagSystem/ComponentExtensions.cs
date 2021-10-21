using SAS.Locator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SAS.TagSystem
{
    public static class ComponentExtensions
    {
        public static readonly ServiceLocator serviceLocator = new ServiceLocator();

        private readonly static Dictionary<Type, Func<Component, Type, string, Component>> _componentCreator = new Dictionary<Type, Func<Component, Type, string, Component>>
        {
            { typeof(AddComponentAttribute), (comp, type, tag) => comp.AddComponent(type, tag) },
        };

        private static readonly Dictionary<Type, Func<Component, Type, bool, Component>> _componentFetchers = new Dictionary<Type, Func<Component, Type, bool, Component>>
        {
            { typeof(FieldRequiresSelfAttribute), (comp, type, includeInactive) => comp.GetComponent(type) },
            { typeof(FieldRequiresChildAttribute), (comp, type, includeInactive) => comp.GetComponentInChildren(type, includeInactive) },
            { typeof(FieldRequiresParentAttribute), (comp, type, includeInactive) => comp.GetComponentInParent(type) },
        };

        private static readonly Dictionary<Type, Func<Component, Type, string, bool, Component>> _componentWithTagFetchers = new Dictionary<Type, Func<Component, Type, string, bool, Component>>
        {
            { typeof(FieldRequiresSelfAttribute), (comp, type, tag, includeInactive) => comp.GetComponent(type, tag) },
            { typeof(FieldRequiresChildAttribute), (comp, type, tag, includeInactive) => comp.GetComponentInChildren(type, tag, includeInactive) },
            { typeof(FieldRequiresParentAttribute), (comp, type, tag, includeInactive) => comp.GetComponentInParent(type, tag, includeInactive) },
        };

        private static readonly Dictionary<Type, Func<Component, Type, bool, Component[]>> _componentsFetchers = new Dictionary<Type, Func<Component, Type, bool, Component[]>>
        {
             { typeof(FieldRequiresSelfAttribute), (comp, type, includeInactive) => comp.GetComponents(type) },
             { typeof(FieldRequiresChildAttribute), (comp, type, includeInactive) => comp.GetComponentsInChildren(type, includeInactive) },
             { typeof(FieldRequiresParentAttribute), (comp, type, includeInactive) => comp.GetComponentsInParent(type, includeInactive) },
        };

        private static Dictionary<Type, Func<Component, Type, string, bool, Component[]>> _componentsWithTagFetchers = new Dictionary<Type, Func<Component, Type, string, bool, Component[]>>
        {
             { typeof(FieldRequiresSelfAttribute), (comp, type, tag, includeInactive) => comp.GetComponents(type, tag) },
             { typeof(FieldRequiresChildAttribute), (comp, type, tag, includeInactive) => comp.GetComponentsInChildren(type, tag, includeInactive) },
             { typeof(FieldRequiresParentAttribute), (comp, type, tag, includeInactive) => comp.GetComponentsInParent(type, tag, includeInactive) },
        };

        public static void Initialize(this Component component, object instance = null)
        {
            instance = instance ?? component;
            var allFields = GetAllFields(instance);
            foreach (var field in allFields)
            {
                var requirement = field.GetCustomAttribute<BaseRequiresAttribute>(false);
                if (requirement != null)
                {
                    if (requirement is AddComponentAttribute)
                    {
                        var dependency = default(Component);
                        if (string.IsNullOrEmpty(requirement.tag))
                            dependency = _componentFetchers[typeof(FieldRequiresSelfAttribute)](component, field.FieldType, true);
                        else
                            dependency = _componentWithTagFetchers[typeof(FieldRequiresSelfAttribute)](component, field.FieldType, requirement.tag, true);
                        if (dependency == null)
                            dependency = _componentCreator[requirement.GetType()](component, field.FieldType, requirement.tag);
                        field.SetValue(instance, dependency);
                    }
                    else
                    {
                        if (requirement is BaseRequiresComponent)
                        {
                            var componentRequirement = requirement as BaseRequiresComponent;
                            if (field.FieldType.IsArray)
                            {
                                var elementType = field.FieldType.GetElementType();
                                var dependencies = default(Component[]);
                                if (string.IsNullOrEmpty(requirement.tag))
                                    dependencies = _componentsFetchers[requirement.GetType()](component, elementType, componentRequirement.includeInactive);
                                else
                                    dependencies = _componentsWithTagFetchers[requirement.GetType()](component, elementType, requirement.tag, componentRequirement.includeInactive);

                                field.SetValue(instance, ConvertArray(dependencies, elementType));
                            }
                            else
                            {
                                var dependency = default(Component);
                                if (string.IsNullOrEmpty(requirement.tag))
                                    dependency = _componentFetchers[requirement.GetType()](component, field.FieldType, componentRequirement.includeInactive);
                                else
                                    dependency = _componentWithTagFetchers[requirement.GetType()](component, field.FieldType, requirement.tag, componentRequirement.includeInactive);
                                field.SetValue(instance, dependency);
                            }
                        }
                        else if (requirement is FieldRequiresModelAttribute)
                        {
                            var modelRequirement = requirement as FieldRequiresModelAttribute;
                            if (modelRequirement.optional)
                            {
                                if (serviceLocator.TryGet(field.FieldType, out var service))
                                    field.SetValue(instance, service);
                            }
                            else
                                field.SetValue(instance, serviceLocator.GetOrCreate(field.FieldType, requirement.tag));
                        }
                    }
                }
            }
        }

        private static IEnumerable<FieldInfo> GetAllFields(this object instance)
        {
            var instanceType = instance.GetType();
            var baseType = instanceType.BaseType;

            FieldInfo[] fields = instanceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
