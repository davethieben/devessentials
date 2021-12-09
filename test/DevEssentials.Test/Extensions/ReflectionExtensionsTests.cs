using System.Collections.Generic;
using Essentials.Reflection;
using Xunit;

namespace Essentials.Test.Extensions
{
    public class ReflectionExtensionsTests
    {
        [Fact]
        public void CopyToDictionary_AllowsNullSource()
        {
            object? source = null;
            var target = new Dictionary<string, object?>();

            source.CopyToDictionary(target);

            Assert.Empty(target);
        }

        [Fact]
        public void CopyToDictionary_SourceIsDictionary()
        {
            var source = new Dictionary<string, object>();
            var target = new Dictionary<string, object?>();

            source.Add("one", 1);
            source.Add("two", 2);

            source.CopyToDictionary(target);

            Assert.Equal(2, target.Count);
        }

        [Fact]
        public void CopyToDictionary_SourceIsList()
        {
            var source = new List<KeyValuePair<string, object>>();
            var target = new Dictionary<string, object?>();

            source.Add(new KeyValuePair<string, object>("one", 1));
            source.Add(new KeyValuePair<string, object>("two", 2));

            source.CopyToDictionary(target);

            Assert.Equal(2, target.Count);
        }

        [Fact]
        public void CopyToDictionary_SourceIsAnonymousObject()
        {
            var target = new Dictionary<string, object?>();

            var source = new
            {
                One = 1,
                Two = "222",
                Three = false
            };

            source.CopyToDictionary(target);

            Assert.Equal(3, target.Count);
        }




    }
}
