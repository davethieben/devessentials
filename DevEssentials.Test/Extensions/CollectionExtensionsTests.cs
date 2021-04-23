using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Essentials;
using Xunit;

namespace DevEssentials.Test.Extensions
{
    public class CollectionExtensionsTests
    {

        [Fact]
        public void IsNullOrEmpty_WhenEmpty()
        {
            var collection = Enumerable.Empty<int>();
            Assert.True(collection.IsNullOrEmpty());
        }

        [Fact]
        public void IsNullOrEmpty_WhenNull()
        {
            int[] collection = null;
            Assert.True(collection.IsNullOrEmpty());
        }

        [Fact]
        public void IsNullOrEmpty_WhenNotEmpty()
        {
            var collection = new[] { 1 };
            Assert.False(collection.IsNullOrEmpty());
        }

    }
}
