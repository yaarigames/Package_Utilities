using System;
using UnityEngine.Scripting;

namespace SAS.TagSystem
{
    [Preserve]
    public class InjectAttribute : BaseRequiresAttribute 
    {
        public bool optional;
        public InjectAttribute(string tag = "", bool optional = false)
        {
            this.optional = optional;
            this.tag = tag;
        }
    }
}