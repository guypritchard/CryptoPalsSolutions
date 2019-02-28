using Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using Xunit;

namespace XUnitTestProject1
{
    public class Set1Tests
    {
        [Fact]
        public void Challenge1()
        {
            var base64 = "49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d"
                            .FromHexStringToBytes()
                            .ToBase64String();

            Assert.Equal("SSdtIGtpbGxpbmcgeW91ciBicmFpbiBsaWtlIGEgcG9pc29ub3VzIG11c2hyb29t", base64);
        }

        [Fact]
        public void Challenge2()
        {
            var source = "1c0111001f010100061a024b53535009181c".FromHexStringToBytes();
            var xor = "686974207468652062756c6c277320657965".FromHexStringToBytes();

            var masked = DataTransformer.Xor(source, xor.Looping());

            Assert.Equal("746865206b696420646f6e277420706c6179", masked.ToHexString());
        }

        [Fact]
        public void Challenge3()
        {
            var winner = DataRecogniser.FindLikelyTextString("1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736");

            Assert.Equal('X', (char)winner.Item1);
        }


        [Fact]
        public void Challenge4()
        {
            var sourceTexts = new List<string>();

            var sourceData = new WebClient().DownloadString("https://cryptopals.com/static/challenge-data/4.txt");

            var sourceText = sourceData.Split(new[] { '\r', '\n' });

            var answer = sourceText.AsParallel().Select(t => DataRecogniser.FindLikelyTextString(t)).OrderByDescending(t => t.Item2).First();

            Assert.Equal('5', (char)answer.Item1);
        }

        [Fact]
        public void Challenge5()
        {
            var bytes = "Burning 'em, if you ain't quick and nimble\nI go crazy when I hear a cymbal".FromByteStringToBytes();

            var d = DataTransformer
                    .Xor(
                        bytes, 
                        "ICE".FromByteStringToBytes().Looping())
                    .ToHexString();

            var expected = "0b3637272a2b2e63622c2e69692a23693a2a3c6324202d623d63343c2a26226324272765272a282b2f20430a652e2c652a3124333a653e2b2027630c692b20283165286326302e27282f";

            Assert.Equal(expected, d);
        }

        [Fact]
        public void Challenge6_1()
        {
            Assert.Equal(37, "this is a test".HammingDistance("wokka wokka!!!"));
        }

        [Fact]
        public void Challenge6_2()
        {
            var sourceTexts = new List<string>();

            var sourceData = new WebClient()
                                .DownloadString("https://cryptopals.com/static/challenge-data/6.txt")
                                .Replace("\n", string.Empty)
                                .FromBase64String();

            var sourceStream = new MemoryStream(sourceData);
            int maxKeySize = 40;

            List<Tuple<int, float>> keyLengths = new List<Tuple<int, float>>();

            var orderedList =
                Enumerable.Range(2, maxKeySize)
                      .Select(keyLength =>
                      {
                          var blocks = new List<byte[]>();
                          for (int i = 0; i <= 16; i++)
                          {
                              blocks.Add(sourceData.Skip(i * keyLength).Take(keyLength).ToArray());
                          }

                          var firstBlock = blocks.First();
                          var hammingDistance = blocks.Skip(1).Sum(b => firstBlock.HammingDistance(b));

                          return Tuple.Create(keyLength, (float)(hammingDistance) / (float)(keyLength), hammingDistance);
                      })
                      .OrderBy(t => t.Item2)
                      .Take(1)
                      .ToList();

           var key = new List<char>();

            var answers = new List<List<Tuple<char, int, string>>>();

           foreach (var ordered in orderedList)
            {
                List<byte>[] datas = new List<byte>[ordered.Item1];

                int count = 0;
                foreach (byte data in sourceData)
                {
                    if (datas[count % ordered.Item1] == null)
                    {
                        datas[count % ordered.Item1] = new List<byte>();
                    }

                    datas[count % ordered.Item1].Add(data);
                    count++;
                }

                answers.Add(datas.Select(d => DataRecogniser.FindLikelyTextString(d)).ToList());
            }

            var likelyKey = answers.OrderByDescending(a => a.Sum(i => i.Item2)).First().Select(i => (byte)i.Item1);

            var decypted = DataTransformer.Xor(sourceData, likelyKey.Looping());

            var final = decypted.ToArray().ToPlainTextString();

            Assert.Contains("Vanilla", final);
        }

        [Fact]
        public void Challenge7()
        {
            var sourceData = new WebClient()
                               .DownloadString("https://cryptopals.com/static/challenge-data/7.txt")
                               .Replace("\n", string.Empty)
                               .FromBase64String();

            var memoryStream = new MemoryStream(sourceData);

            var key = "YELLOW SUBMARINE".FromByteStringToBytes().ToArray();

            string plainText = string.Empty;

            using (AesManaged aes = new AesManaged())
            {
                aes.Padding = PaddingMode.None;
                aes.Mode = CipherMode.ECB;

                var decryptor = aes.CreateDecryptor(key, new byte[16]);

                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cryptoStream))
                    {
                        plainText = reader.ReadToEnd();
                    }
                }
            }

            Assert.Contains("funky music", plainText);
        }

        [Fact]
        public void Challenge8()
        {
            var sourceData = new WebClient()
                             .DownloadString("https://cryptopals.com/static/challenge-data/8.txt")
                             .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                             .Select(s => s.FromBase64String());

            int keyLength = 16;

            var result = sourceData.Select((data, j) =>
            {
                var blocks = new List<string>();
                for (int i = 0; i < data.Length / keyLength; i++)
                {
                    blocks.Add(data.Skip(i * keyLength).Take(keyLength).ToArray().ToBase64String());
                }

                return Tuple.Create(data.ToBase64String(), blocks.Distinct().Count());
            }).OrderBy(t => t.Item2).First();

            Assert.StartsWith("d88", result.Item1);
        }
    }
}
