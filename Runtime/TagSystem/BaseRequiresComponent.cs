using System;

namespace SAS.TagSystem
{
    public abstract class BaseRequiresComponent : Attribute
    {
        public string tag;
        public bool includeInactive;
    }
}
