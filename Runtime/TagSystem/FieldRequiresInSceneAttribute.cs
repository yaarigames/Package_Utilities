using System;

namespace SAS.Utilities.TagSystem
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldRequiresInSceneAttribute : BaseRequiresComponent
    {
        public FieldRequiresInSceneAttribute(Tag tag = Tag.None, bool includeInactive = false)
        {
            this.includeInactive = includeInactive;
            this.tag = tag;
        }
    }
}
