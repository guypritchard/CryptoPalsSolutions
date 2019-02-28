using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Looping<T>(this IEnumerable<T> data)
        {
            while (true)
            {
                foreach (var item in data)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }
    }

}
