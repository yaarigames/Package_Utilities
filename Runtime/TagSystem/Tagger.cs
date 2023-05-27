using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAS.Utilities.TagSystem
{
    [DisallowMultipleComponent()]
	public class Tagger : MonoBehaviour
	{
		[Serializable]
		public class Tag
		{
			[SerializeField] private Component m_Component;
			[SerializeField] private TagSystem.Tag m_Value;

			public Component Component => m_Component;

			public  TagSystem.Tag Value { get => m_Value; set => m_Value = value; }

			public Tag(Component component, TagSystem.Tag val)
			{
				m_Component = component;
				m_Value = val;
			}
		}

		[SerializeField] private List<Tag> m_Tags = new List<Tag>();

		public IEnumerable<Component> Find<T>(TagSystem.Tag tag) where T : Component
		{
			return m_Tags.Where(item => item.Value == tag && item.Component.GetType() == typeof(T)).Select(item => item.Component);
		}

		public IEnumerable<Component> Find(Type type, TagSystem.Tag tag)
		{
			return m_Tags.Where(item => item.Value == tag && item.Component.GetType() == type).Select(item => item.Component);
		}

		public Tag Find(Component component)
		{
			return m_Tags.FirstOrDefault(tag => tag.Component == component);
		}

		public bool HasTag(Component component, TagSystem.Tag tag)
        {
			return m_Tags.Find(item => item.Component == component && item.Value == tag) != null;		
		}

		public string GetTag(Component component)
		{
			var tag = m_Tags.Find(ele => ele.Component == component);
			return tag?.Value.ToString();
		}

		public void AddTag(Component component, TagSystem.Tag tagValue = TagSystem.Tag.None)
		{
			var tag = m_Tags.Find(ele => ele.Component == component);
			if (tag == null || tag.Value != tagValue)
                m_Tags.Add(new Tag(component, TagSystem.Tag.None));
		}

		public void RemoveTag(Component component)
		{
			var tag = m_Tags.Find(ele => ele.Component == component);
			m_Tags.Remove(tag);
		}

		public void RemoveAllTags(Component component)
		{
			var tags = m_Tags.FindAll(ele => ele.Component == component);
			foreach (var tag in tags)
				m_Tags.Remove(tag);
		}
	}
}