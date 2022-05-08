using System;

namespace SAS.TagSystem
{
    public interface IContext
    {
        object GetOrCreate(Type type, string tag = "");
        bool TryGet(Type type, out object instance, string tag = "");
    }
}
