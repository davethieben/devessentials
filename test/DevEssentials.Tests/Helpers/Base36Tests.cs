using System.Text;
using Xunit;

namespace Essentials.Test.Helpers
{
    public class Base36Tests
    {
        [Fact]
        public void Encode01()
        {
            var input = new byte[] { 1 };

            string output = Base36.Encode(input);

            Assert.NotEmpty(output);
            Assert.Equal("1", output);
        }

        [Fact]
        public void Encode02()
        {
            var input = new byte[] { 1, 2, 3, 4 };

            string output = Base36.Encode(input);

            Assert.NotEmpty(output);
            Assert.Equal("142lmp", output);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("a")]
        [InlineData("one1")]
        [InlineData("The Quick Brown Fox Jumped Over The Lazy Brown Dogs!@#$^&*?")]
        public void OutputSameAsInput(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);

            string encoded = Base36.Encode(bytes);

            bytes = Base36.Decode(encoded);
            string output = Encoding.UTF8.GetString(bytes);

            Assert.Equal(input, output);
        }

    }
}
