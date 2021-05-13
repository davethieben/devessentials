using System;
using System.Collections.Generic;
using System.Linq;

namespace Essentials
{
    public static class DateExtensions
    {
        public static string? ToShortDateString(this DateTime? input) =>
            input.HasValue ?
                input.Value.ToString("d") :
                null as string;

        public static bool HasPassed(this DateTimeOffset dateTime) => 
            dateTime <= DateTimeOffset.UtcNow;

        public static bool IsWithin(this DateTimeOffset dateTime, TimeSpan timeSpan) =>
            (dateTime - timeSpan).HasPassed();

    }
}
