using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Essentials;
using Xunit;

namespace Essentials.Test.Extensions
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
            int[]? collection = null;
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

        [Fact]
        public void Remove_NullPredicate()
        {
            var list = new List<TestDto>
            {
            };

            Assert.Throws<ContractException>(() =>
            {
                Predicate<TestDto>? predicate = null;

                list.Remove(predicate!);
            });
        }

        [Fact]
        public void EmptyIfNull_WhenNull_ReturnsNonNull()
        {
            IEnumerable<TestDto>? list = null;

            var result = list.EmptyIfNull();
            Assert.NotNull(result);
        }

        [Fact]
        public void EmptyIfNull_WhenNotNull_ReturnsOriginal()
        {
            var list = new List<TestDto>
            {
                new TestDto { Id = 1, Name = "AAA" },
                new TestDto { Id = 2, Name = "BBB" },
                new TestDto { Id = 3, Name = "CCC" },
                new TestDto { Id = 4, Name = "DDD" },
                new TestDto { Id = 5, Name = "CCC" },
            };

            var result = list.EmptyIfNull();
            Assert.NotNull(result);
            Assert.Same(list[0], result.ElementAt(0));
        }

        [Fact]
        public void IsEqualTo_WhenStringsEqual_ReturnsTrue()
        {
            var first = new List<string>
            {
                "one", "two", "three"
            };

            var second = new List<string>
            {
                "one", "two", "three"
            };

            Assert.True(first.IsEqualTo(second));
        }

        [Fact]
        public void IsEqualTo_WhenNull_ReturnsTrue()
        {
            List<string>? first = null;
            List<string>? second = null;

            Assert.True(first.IsEqualTo(second));
        }

        [Fact]
        public void IsEqualTo_WhenStringsNotEqual_ReturnsFalse()
        {
            var first = new List<string>
            {
                "one", "two", "three"
            };

            var second = new List<string>
            {
                "one", "two", "four"
            };

            Assert.False(first.IsEqualTo(second));
        }

        [Fact]
        public void IsEqualTo_WhenNullNotEqual_ReturnsFalse()
        {
            var first = new List<string>
            {
                "one", "two", "three"
            };

            List<string>? second = null;

            Assert.False(first.IsEqualTo(second));
        }

        [Fact]
        public void IsEqualTo_WhenObjectsEqual_ReturnsTrue_DefaultComparer()
        {
            var one = new TestDto { Id = 1, Name = "one" };
            var two = new TestDto { Id = 2, Name = "two" };
            var three = new TestDto { Id = 3, Name = "three" };

            var first = new List<TestDto>
            {
                one, two, three
            };

            var second = new List<TestDto>
            {
                one, two, three
            };

            Assert.True(first.IsEqualTo(second));
        }

        [Fact]
        public void IsEqualTo_WhenObjectsNotEqual_ReturnsFalse_DefaultComparer()
        {
            var one = new TestDto { Id = 1, Name = "one" };
            var two = new TestDto { Id = 2, Name = "two" };
            var three = new TestDto { Id = 3, Name = "three" };
            var four = new TestDto { Id = 4, Name = "four" };

            var first = new List<TestDto>
            {
                one, two, three
            };

            var second = new List<TestDto>
            {
                one, two, four
            };

            Assert.False(first.IsEqualTo(second));
        }

        private class TestDto
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        private class TestDtoComparer : KeyComparer<TestDto>
        {
            public TestDtoComparer() : base(dto => dto.Id) { }
        }

    }
}
