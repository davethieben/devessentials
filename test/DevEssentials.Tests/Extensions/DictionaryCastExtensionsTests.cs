using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Essentials.Test.Extensions
{
    public class DictionaryCastExtensionsTests
    {
        [Fact]
        public void WhenCastCanRead()
        {
            var hashtable = new Hashtable();
            hashtable.Add("one", 1);
            hashtable.Add("two", 2);

            var cast = hashtable.Cast<string, int>();

            // IDictionary<K,V>.Count:
            Assert.Equal(2, cast.Count);
            // IDictionary<K,V>.Indexer:
            Assert.Equal(1, cast["one"]);
            Assert.Equal(2, cast["two"]);

            // IDictionary<K,V>.Keys:
            ICollection<string>? keys = cast.Keys;
            Assert.NotNull(keys);
            Assert.Contains("one", keys);
            Assert.Contains("two", keys);

            // IDictionary<K,V>.Values:
            ICollection<int>? values = cast.Values;
            Assert.NotNull(values);
            Assert.Contains(1, values);
            Assert.Contains(2, values);

            // IDictionary<K,V>.ContainsKey:
            bool containsKey = cast.ContainsKey("one");
            Assert.True(containsKey);
            containsKey = cast.ContainsKey("notInHashtable");
            Assert.False(containsKey);

            // IDictionary<K,V>.Contains:
            bool contains = cast.Contains(new KeyValuePair<string, int>("two", 2));
            Assert.True(contains);
            contains = cast.Contains(new KeyValuePair<string, int>("notInHashtable", 2));
            Assert.False(contains);
            contains = cast.Contains(new KeyValuePair<string, int>("two", 2222));
            Assert.False(contains);
        }

        [Fact]
        public void WhenCastCanEnumerate()
        {
            var hashtable = new Hashtable();
            hashtable.Add("one", 1);
            hashtable.Add("two", 2);
            hashtable.Add("three", 3);
            hashtable.Add("four", 4);

            var cast = hashtable.Cast<string, int>();
            var castEnum = cast.GetEnumerator();

            int didEnumerateCount = 0;
            while (castEnum.MoveNext())
            {
                didEnumerateCount++;
                Assert.True(hashtable.ContainsKey(castEnum.Current.Key));
            }

            Assert.Equal(4, didEnumerateCount);
        }

        [Fact]
        public void WhenCastCanAdd()
        {
            var hashtable = new Hashtable();
            var cast = hashtable.Cast<string, int>();

            cast.Add("one", 1);
            Assert.Single(hashtable);

            cast.Add("two", 2);
            Assert.Equal(2, hashtable.Count);

            Assert.Equal(1, hashtable["one"]);
            Assert.Equal(2, hashtable["two"]);

            // KeyValuePair:
            cast.Add(new KeyValuePair<string, int>("three", 3));
            Assert.Equal(3, hashtable.Count);
            Assert.Equal(3, hashtable["three"]);

        }

        [Fact]
        public void WhenCastCanRemove()
        {
            var hashtable = new Hashtable();
            hashtable.Add("one", 1);
            hashtable.Add("two", 2);
            hashtable.Add("three", 3);
            hashtable.Add("four", 4);

            var cast = hashtable.Cast<string, int>();
            bool result = cast.Remove("three");
            Assert.True(result);
            Assert.Equal(3, hashtable.Count);
            Assert.False(hashtable.ContainsKey("three"));

            // KeyValuePair:
            result = cast.Remove(new KeyValuePair<string, int>("four", 4));
            Assert.True(result);
            Assert.Equal(2, hashtable.Count);
            Assert.False(hashtable.ContainsKey("four"));

            // not removed:
            result = cast.Remove(new KeyValuePair<string, int>("twotwo", 2));
            Assert.False(result);
            Assert.Equal(2, hashtable.Count);
            Assert.True(hashtable.ContainsKey("two"));
        }

        [Fact]
        public void WhenCastCanClear()
        {
            var hashtable = new Hashtable();
            hashtable.Add("one", 1);
            hashtable.Add("two", 2);
            hashtable.Add("three", 3);
            hashtable.Add("four", 4);

            var cast = hashtable.Cast<string, int>();
            cast.Clear();

            Assert.Empty(hashtable);
            Assert.Empty(cast);
        }

    }
}
