using System.Linq;
using UnityEngine;
using System;

namespace SAS.TagSystem
{
    public static class TaggerExtensions
    {
        public static Component AddComponent(this Component component, Type type, string tag = "")
        {
            var addedComponent = component.gameObject.AddComponent(type);

            if (!string.IsNullOrEmpty(tag))
            {
                var tagger = component.GetComponent<Tagger>();
                if (tagger == null)
                    tagger = component.gameObject.AddComponent<Tagger>();
                tagger.AddTag(addedComponent, tag);
            }

            return addedComponent;
        }

        public static T AddComponent<T>(this Component component, string tag = "")
        {
            return (T)(object)component.AddComponent(typeof(T), tag); 
        }

        public static T GetComponent<T>(this Component component, string tag)
        {
            return (T)(object)component.GetComponent(typeof(T), tag);
        }

        public static Component GetComponent(this Component component, Type type, string tag)
        {
            return GetComponentByTag(component.GetComponents(type), tag);
        }

        public static Component[] GetComponents(this Component component, Type type, string tag)
        {
            return GetComponentsByTag(component.GetComponents(type), tag);
        }

        public static T GetComponentInChildren<T>(this Component component, string tag, bool includeInactive = false)
        {
            return (T)(object)component.GetComponentInChildren(typeof(T), tag, includeInactive);
        }

        public static Component GetComponentInChildren(this Component component, Type type, string tag, bool includeInactive = false)
        {
            return GetComponentByTag(component.GetComponentsInChildren(type, includeInactive), tag);
        }

        public static T GetComponentInParent<T>(this Component component, string tag, bool includeInactive = false)
        {
            return (T)(object)component.GetComponentsInParent(typeof(T), tag, includeInactive);
        }

        public static Component GetComponentInParent(this Component component, Type type, string tag, bool includeInactive = false)
        {
            return GetComponentByTag(component.GetComponentsInParent(type, includeInactive), tag);
        }

        public static T[] GetComponentsInParent<T>(this Component component, string tag, bool includeInactive = false)
        {
            return (T[])(object)component.GetComponentInParent(typeof(T), tag, includeInactive);
        }

        public static Component[] GetComponentsInParent(this Component component, Type type, string tag, bool includeInactive = false)
        {
            return GetComponentsByTag(component.GetComponentsInParent(type, includeInactive), tag);
        }

        public static T[] GetComponentsInChildren<T>(this Component component, string tag, bool includeInactive = false)
        {
            return (T[])(object)component.GetComponentInChildren(typeof(T), tag, includeInactive);
        }

        public static Component[] GetComponentsInChildren(this Component component, Type type, string tag, bool includeInactive = false)
        {
            return GetComponentsByTag(component.GetComponentsInChildren(type, includeInactive), tag);
        }

        private static T GetComponentByTag<T>(T[] components, string tag) where T : Component
        {
            if (string.IsNullOrEmpty(tag))
                return components.FirstOrDefault();
            else
                return components.FirstOrDefault(component => component.GetComponent<Tagger>()?.Find(component)?.Value == tag);
        }

        private static T[] GetComponentsByTag<T>(T[] components, string tag) where T : Component
        {
            if (string.IsNullOrEmpty(tag))
                return components;
            else
                return components.Where(component => component.GetComponent<Tagger>()?.Find(component)?.Value == tag).ToArray();
        }

        public static string GetTag(this Component component)
        {
            return component.GetComponent<Tagger>()?.Find(component)?.Value;
        }
    }
}
