using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Crypto
{
    public static class ByteExtensions
    {
        public static IEnumerable<byte> Pad(this IEnumerable<byte> data, int blockLength)
        {
            var remainder = blockLength - (data.Count() % blockLength);

            if (remainder != 0)
            {
                return data.Concat(Enumerable.Repeat<byte>(0x04, remainder));
            }

            return data;
        }
    }
}
