using System;

namespace SAS.Utilities.TagSystem
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
