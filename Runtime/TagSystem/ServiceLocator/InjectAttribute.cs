using UnityEngine.Scripting;

namespace SAS.Utilities.TagSystem
{
    [Preserve]
    public class InjectAttribute : BaseRequiresAttribute 
    {
        public bool optional;
        public InjectAttribute(Tag tag = Tag.None, bool optional = false)
        {
            this.optional = optional;
            this.tag = tag;
        }
    }
}