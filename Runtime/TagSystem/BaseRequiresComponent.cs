using System;

namespace SAS.TagSystem
{
    public abstract class BaseRequiresComponent : BaseRequiresAttribute
    {
        public bool includeInactive;
    }
    public abstract class BaseRequiresAttribute : Attribute
    {
        public string tag;
    }
}
