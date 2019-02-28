using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto
{
    public static class DataTransformer
    {
        public static IEnumerable<byte> Xor(IEnumerable<byte> src, IEnumerable<byte> mask)
        {
            var maskEnumerator = mask.GetEnumerator();

            foreach (byte byteData in src)
            {
                maskEnumerator.MoveNext();
                yield return (byte)(byteData ^ maskEnumerator.Current);
            }
        }
    }
}
