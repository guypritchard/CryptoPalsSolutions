using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crypto
{
    // https://cryptopals.com/sets/1/challenges/1

    public static class StringEncoderDecoder
    {
        public static int HammingDistance(this string first, string second)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            if (first.Length != second.Length)
            {
                throw new ArgumentException("Strings must have the same length.");
            }

            return HammingDistance(
                first.FromByteStringToBytes().ToArray(), 
                second.FromByteStringToBytes().ToArray());
        }

        public static int HammingDistance(this IEnumerable<byte> first, IEnumerable<byte> second)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            if (first.Count() != second.Count())
            {
                throw new ArgumentException("Strings must have the same length.");
            }

            var firstArray = new BitArray(first.ToArray());
            var secondArray = new BitArray(second.ToArray());

            int differences = 0;

            for (int i = 0; i < firstArray.Length; i++)
            {
                if (firstArray[i] != secondArray[i])
                {
                    differences++;
                }
            }

            return differences;
        }

        public static IEnumerable<byte> FromHexStringToBytes(this string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
            {
                throw new ArgumentException("hexString null or empty");
            }

            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("hexString length not divisible by 2.");
            }

            for (int i = 0; i < hexString.Length; i+=2)
            {
                var charPair = hexString.Substring(i, 2);
                yield return Convert.ToByte(charPair, 16);
            }
        }

        public static IEnumerable<byte> FromByteStringToBytes(this string data)
        {
            return data.ToCharArray().Select(c => (byte)c);
        }

        public static string ToBase64String (this IEnumerable<byte> bytes)
        {
            if (bytes == null || !bytes.Any())
            {
                throw new ArgumentException(nameof(bytes));
            }

            return Convert.ToBase64String(bytes.ToArray());
        }

        public static byte[] FromBase64String(this string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                throw new ArgumentException(nameof(base64));
            }

            return Convert.FromBase64String(base64);
        }

        public static string ToHexString(this IEnumerable<byte> bytes)
        {
            if (bytes == null || !bytes.Any())
            {
                throw new ArgumentException(nameof(bytes));
            }

            return string.Join(string.Empty, bytes.Select(b => b.ToString("x2")));
        }

        public static string ToPlainTextString(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentException(nameof(bytes));
            }

            StringBuilder sb = new StringBuilder();
            foreach(var data in bytes)
            {
                sb.Append((char)data);
            }

            return sb.ToString();
        }
    }
}
