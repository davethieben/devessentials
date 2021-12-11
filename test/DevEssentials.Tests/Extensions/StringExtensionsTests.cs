using System;
using Xunit;

namespace Essentials.Test.Extensions
{
    public class StringExtensionsTests
    {

        [Fact]
        public void Left_GetsLeft()
        {
            string input = "string test";

            string output = input.Left(5);
            Assert.Equal("strin", output);
        }

        [Fact]
        public void Left_IsSafe()
        {
            string input = "string";

            string output = input.Left(10);
            Assert.Equal("string", output);
        }

        [Fact]
        public void Left_IsSafeForNull()
        {
            string? input = null;

            string output = input.Left(10);
            Assert.Equal("", output);
        }

        [Fact]
        public void Right_GetsRight()
        {
            string input = "string test";

            string output = input.Right(5);
            Assert.Equal(" test", output);
        }

        [Fact]
        public void Right_IsSafe()
        {
            string input = "string";

            string output = input.Left(10);
            Assert.Equal("string", output);
        }

        [Fact]
        public void Right_IsSafeForNull()
        {
            string? input = null;

            string output = input.Right(10);
            Assert.Equal("", output);
        }


    }
}
