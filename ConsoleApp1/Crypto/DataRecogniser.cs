using System;
using System.Collections.Generic;
using System.Linq;

namespace Crypto
{
    public static class DataRecogniser
    {
        /// <summary>
        /// http://pi.math.cornell.edu/~mec/2003-2004/cryptography/subs/frequencies.html
        /// </summary>
        public static bool IsEnglish(byte[] data)
        {
            return data.All(d => IsEnglish(d));
        }

        public static bool IsEnglish(byte letter)
        {
            return (letter >= 'A' && letter <= 'Z') ||
                   (letter >= 'a' && letter <= 'z') ||
                   letter == ' ';
        }

        public static Tuple<char, int, string> FindLikelyTextString(string data)
        {
            var encodedString = data.FromHexStringToBytes();

            return FindLikelyTextString(encodedString);
        }

        public static Tuple<char, int, string> FindLikelyTextString(IEnumerable<byte> data)
        {
            var scores = new List<Tuple<char, int, string>>();
            for (int i = 0; i < 255; i++)
            {
                byte mask = (byte)i;

                var output = DataTransformer.Xor(data, mask.ToEnumerable().Looping()).ToArray();
                int countChars = output.Count(c => DataRecogniser.IsEnglish(c));

                scores.Add(Tuple.Create(((char)mask), countChars, output.ToPlainTextString()));
            }

            var potentialAnswers = scores.OrderByDescending(t => t.Item2);

            return potentialAnswers.First();
        }
    }
}
