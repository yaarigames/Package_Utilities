using System;
using UnityEngine.Scripting;

namespace SAS.TagSystem
{
    [Preserve]
    public class FieldRequiresModelAttribute : BaseRequiresAttribute 
    {
        public bool optional;
    }
}