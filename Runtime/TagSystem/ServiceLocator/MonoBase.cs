using System;
using System.Collections.Generic;
using UnityEngine;

namespace SAS.Utilities.TagSystem
{
    public class MonoBase : MonoBehaviour
    {
        [SerializeField] private bool m_InitializeOnStart = false;

        private List<MonoBase> _children = new List<MonoBase>();
        public IReadOnlyList<MonoBase> Children => _children;

        protected virtual void Awake()
        {
            if (!m_InitializeOnStart)
                this.Initialize();
        }

        protected virtual void Start()
        {
            if (m_InitializeOnStart)
                this.Initialize();
        }

        protected virtual void OnDestroy()
        {
            // TODO: Use refection set sett all the properties null;
            foreach (var child in _children)
            {
                Destroy(child.gameObject);
            }
        }

        internal void SetParent(MonoBase parent)
        {
            parent?.AddChild(this);
        }

        private void AddChild(MonoBase monoBase)
        {
            _children.Add(monoBase);
        }
    }
}
