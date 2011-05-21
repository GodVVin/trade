using System;
using System.Collections.Generic;
using System.Linq;
using Trade.Core.Stock;

namespace Trade.Core.Helpers
{
    public static class EnumerableEx
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static Bar ByTime(this IEnumerable<Bar> bars, DateTime time)
        {
            return bars.FirstOrDefault(bar => bar.Time == time);
        }
    }
}
