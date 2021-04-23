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

        [Fact]
        public void AddRange_AddsMultiples()
        {
            IList<int> list = new List<int>();
            list.AddRange(new[] { 1, 2, 3 });

            Assert.Equal(3, list.Count);
        }

        [Fact]
        public void AddRange_NullAddsNothing()
        {
            IList<int> list = new List<int> { 1, 2, 3 };
            list.AddRange(null);

            Assert.Equal(3, list.Count);
        }

        [Fact]
        public void Remove_RemovesMatches()
        {
            var list = new List<TestDto>
            {
                new TestDto { Id = 1, Name = "AAA" },
                new TestDto { Id = 2, Name = "BBB" },
                new TestDto { Id = 3, Name = "CCC" },
                new TestDto { Id = 4, Name = "DDD" },
                new TestDto { Id = 5, Name = "CCC" },
            };

            list.Remove(x => x.Name == "CCC");
            Assert.Equal(3, list.Count);
        }



        private class TestDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
