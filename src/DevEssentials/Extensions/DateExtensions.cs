using System;
using System.Collections.Generic;
using System.Linq;

namespace Essentials
{
    public static class DateExtensions
    {
        private static SortedList<double, Func<TimeSpan, string>> _relativeDescriptions = new SortedList<double, Func<TimeSpan, string>>();

        static DateExtensions()
        {
            _relativeDescriptions.Add(0.75, (relativeSpan) => "less than a minute");
            _relativeDescriptions.Add(1.5, (relativeSpan) => "about a minute");
            _relativeDescriptions.Add(45, (relativeSpan) => $"{Math.Round(relativeSpan.TotalMinutes)} minutes");
            _relativeDescriptions.Add(90, (relativeSpan) => "about an hour");
            _relativeDescriptions.Add(1440, (relativeSpan) => $"about {Math.Round(Math.Abs(relativeSpan.TotalHours))} hours"); // 60 * 24
            _relativeDescriptions.Add(2880, (relativeSpan) => "a day"); // 60 * 48
            _relativeDescriptions.Add(43200, (relativeSpan) => $"{Math.Floor(Math.Abs(relativeSpan.TotalDays))} days"); // 60 * 24 * 30
            _relativeDescriptions.Add(86400, (relativeSpan) => "about a month"); // 60 * 24 * 60
            _relativeDescriptions.Add(525600, (relativeSpan) => $"{Math.Floor(Math.Abs(relativeSpan.TotalDays / 30))} months"); // 60 * 24 * 365 
            _relativeDescriptions.Add(1051200, (relativeSpan) => "about a year"); // 60 * 24 * 365 * 2
            _relativeDescriptions.Add(double.MaxValue, (relativeSpan) => $"{Math.Floor(Math.Abs(relativeSpan.TotalDays / 365))} years");

        }

        public static string ToString(this DateTime? input, string format) =>
            input.HasValue ? input.Value.ToString(format) : string.Empty;

        public static string ToString(this DateTimeOffset? input, string format) =>
            input.HasValue ? input.Value.ToString(format) : string.Empty;

        public static string RelativeDate(this DateTime input) => RelativeDate(DateTime.UtcNow.Subtract(input));

        public static string RelativeDate(this DateTimeOffset input) => RelativeDate(DateTimeOffset.UtcNow.Subtract(input));

        public static string RelativeDate(this TimeSpan relativeSpan)
        {
            double totalMinutes = relativeSpan.TotalMinutes;
            var suffix = " ago";

            if (totalMinutes < 0.0)
            {
                totalMinutes = Math.Abs(totalMinutes);
                suffix = " from now";
            }

            return _relativeDescriptions.First(n => totalMinutes < n.Key).Value.Invoke(relativeSpan) + suffix;
        }

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
