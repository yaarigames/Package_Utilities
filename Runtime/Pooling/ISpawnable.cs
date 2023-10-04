using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAS.Pool
{
    public interface ISpawnable
    {
        void OnSpawn<T>(T obj);
        void OnDespawn();
    }
}
