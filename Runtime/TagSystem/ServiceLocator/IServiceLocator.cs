using System;
using System.Collections.Generic;

namespace SAS.TagSystem
{
    public interface IServiceLocator
    {
        T Get<T>(string tag = "");
        bool TryGet<T>(out T service, string tag = "");
        bool TryGet(Type type, out object service, string tag = "");
        IEnumerable<T> GetAll<T>(string tag = "");
    }
}
