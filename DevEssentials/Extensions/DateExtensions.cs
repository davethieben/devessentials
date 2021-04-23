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

        public static string ToShortestString(this TimeSpan span)
        {
            string format =
                (span.Days > 0 ? "d\\:" : "") +
                (span.Hours > 0 ? "h\\:" : "") +
                "mm\\:ss";

            return span.ToString(format);
        }

        public static string ToReadableString(this TimeSpan span)
        {
            return string.Join(", ", span.GetReadableStringElements()
               .Where(str => !string.IsNullOrWhiteSpace(str)));
        }

        private static IEnumerable<string> GetReadableStringElements(this TimeSpan span)
        {
            yield return GetDaysString((int)Math.Floor(span.TotalDays));
            yield return GetHoursString(span.Hours);
            yield return GetMinutesString(span.Minutes);
            yield return GetSecondsString(span.Seconds);
        }

        private static string GetDaysString(int days)
        {
            if (days == 0)
                return string.Empty;

            if (days == 1)
                return "1 day";

            return string.Format("{0:0} days", days);
        }

        private static string GetHoursString(int hours)
        {
            if (hours == 0)
                return string.Empty;

            if (hours == 1)
                return "1 hour";

            return string.Format("{0:0} hours", hours);
        }

        private static string GetMinutesString(int minutes)
        {
            if (minutes == 0)
                return string.Empty;

            if (minutes == 1)
                return "1 minute";

            return string.Format("{0:0} minutes", minutes);
        }

        private static string GetSecondsString(int seconds)
        {
            if (seconds == 0)
                return string.Empty;

            if (seconds == 1)
                return "1 second";

            return string.Format("{0:0} seconds", seconds);
        }
    }
}
