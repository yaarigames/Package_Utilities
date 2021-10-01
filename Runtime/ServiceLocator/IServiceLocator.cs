using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAS.Locator
{
    public interface IServiceLocator
    {
        T Get<T>(string tag = "");
        bool TryGet<T>(out T service, string tag = "");
        bool TryGet(Type type, out object service, string tag = "");
       
    }
}
