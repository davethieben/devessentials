using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Essentials.Test.Helpers
{
    public class StringReplacementTests
    {
        [Fact]
        public void ReplacementMatchesExpected()
        {
            var replacer = new StringReplacement("Hello {test} world!");
            replacer.Add("test", "-cruel-");

            var output = replacer.ToString();
            Assert.Equal("Hello -cruel- world!", output);
        }


    }
}
