using System;
using System.Collections.Generic;

namespace SAS.Async
{
    public static class Extensions
    {
        public static IEnumerable<Routine> ToRoutiens<T>(this IEnumerable<T> items, Func<T, Routine> toYield)
        {
            var yields = new List<Routine>();
            foreach (var item in items)
                yields.Add(toYield(item));

            return yields;
        }
    }
}
