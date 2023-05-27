using System;

namespace SAS.Utilities.TagSystem
{
    public interface IContext
    {
        object GetOrCreate(Type type, Tag tag = Tag.None);
        bool TryGet(Type type, out object instance, Tag tag = Tag.None);
    }
}
