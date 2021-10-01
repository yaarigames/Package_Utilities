using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAS.Pool
{
    public abstract class FactorySO<T> : ScriptableObject, IFactory<T>
    {
        public abstract bool Create(out T item);
    }
}
