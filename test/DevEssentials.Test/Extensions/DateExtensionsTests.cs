using System;
using Xunit;

namespace Essentials.Test.Extensions
{
    public class DateExtensionsTests
    {
        [Fact]
        public void Relative_SecondFromNow()
        {
            var startTime = DateTimeOffset.UtcNow.AddSeconds(1);

            string result = startTime.RelativeDate();
            Assert.Contains("less than", result);
            Assert.Contains("from now", result);
        }

        [Fact]
        public void Relative_SecondAgo()
        {
            var startTime = DateTimeOffset.UtcNow.AddSeconds(-1);

            string result = startTime.RelativeDate();
            Assert.Contains("less than", result);
            Assert.Contains("ago", result);
        }

        [Fact]
        public void Relative_MinuteFromNow()
        {
            var startTime = DateTimeOffset.UtcNow.AddMinutes(1);

            string result = startTime.RelativeDate();
            Assert.Contains("minute", result);
            Assert.Contains("from now", result);
        }

        [Fact]
        public void Relative_MinuteAgo()
        {
            var startTime = DateTimeOffset.UtcNow.AddMinutes(-1);

            string result = startTime.RelativeDate();
            Assert.Contains("minute", result);
            Assert.Contains("ago", result);
        }




    }
}
