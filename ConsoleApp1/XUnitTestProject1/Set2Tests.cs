using Crypto;
using System.Linq;
using Xunit;

namespace XUnitTestProject1
{
    public class Set2Tests
    {
        [Fact]
        public void Challenge9()
        {
            int blockLength = 20;

            var data = "YELLOW SUBMARINE".FromByteStringToBytes();

            var padded = data.Pad(blockLength).ToArray().ToPlainTextString();

            Assert.Equal("YELLOW SUBMARINE\u0004\u0004\u0004\u0004", padded);
        }
    }
}
