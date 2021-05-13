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

        public static string RelativeDate(this DateTime input)
        {
            var relativeSpan = DateTime.UtcNow.Subtract(input);
            var totalMinutes = relativeSpan.TotalMinutes;
            var suffix = " ago";

            if (totalMinutes < 0.0)
            {
                totalMinutes = Math.Abs(totalMinutes);
                suffix = " from now";
            }

            var aValue = new SortedList<double, Func<string>>();
            aValue.Add(0.75, () => "less than a minute");
            aValue.Add(1.5, () => "about a minute");
            aValue.Add(45, () => $"{Math.Round(totalMinutes)} minutes");
            aValue.Add(90, () => "about an hour");
            aValue.Add(1440, () => $"about {Math.Round(Math.Abs(relativeSpan.TotalHours))} hours"); // 60 * 24
            aValue.Add(2880, () => "a day"); // 60 * 48
            aValue.Add(43200, () => $"{Math.Floor(Math.Abs(relativeSpan.TotalDays))} days"); // 60 * 24 * 30
            aValue.Add(86400, () => "about a month"); // 60 * 24 * 60
            aValue.Add(525600, () => $"{Math.Floor(Math.Abs(relativeSpan.TotalDays / 30))} months"); // 60 * 24 * 365 
            aValue.Add(1051200, () => "about a year"); // 60 * 24 * 365 * 2
            aValue.Add(double.MaxValue, () => $"{Math.Floor(Math.Abs(relativeSpan.TotalDays / 365))} years");
         
            return aValue.First(n => totalMinutes < n.Key).Value.Invoke() + suffix;
        }

    }
}
