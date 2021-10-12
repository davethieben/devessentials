using System;
using System.Linq;

namespace Essentials
{
    public static class NumberExtensions
    {
        public static string ToKbString(this long numBytes, int numDecimalPlaces = 0)
        {
            double kb = (double)numBytes / 1024;
            return kb.ToString($"n{numDecimalPlaces}");
        }

        public static string ToMbString(this long numBytes, IFormatProvider? formatter = null) =>
            numBytes.ToMegabytes().ToString("n1", formatter);

        /// <summary>
        /// since Windows reports capacity in MiB, that's what we're going to use here:
        ///     https://en.wikipedia.org/wiki/Megabyte
        /// </summary>
        public static long ToMegabytes(this long numBytes) => (long)(numBytes / 1048576d);

        public static string ToGbString(this long numBytes)
        {
            // since Windows reports capacity in GiB, that's what we're going to use here:
            //      https://en.wikipedia.org/wiki/Gigabyte
            double gb = (double)numBytes / 1073741824;
            return gb.ToString("n1");
        }

        public static int[] ToIntArray(this string csv, params char[]? separators)
        {
            if (separators.IsNullOrEmpty())
                separators = new[] { ',' };

            return !string.IsNullOrEmpty(csv)
                ? csv.Split(separators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToInt()).ToArray()
                : new int[0];
        }

        public static string ToNthString(this int number)
        {
            string suffix;
            if (number % 100 == 11 || number % 100 == 12 || number % 100 == 13)
            {
                suffix = "th";
            }
            else if (number % 10 == 1)
            {
                suffix = "st";
            }
            else if (number % 10 == 2)
            {
                suffix = "nd";
            }
            else if (number % 10 == 3)
            {
                suffix = "rd";
            }
            else
            {
                suffix = "th";
            }
            return $"{number}{suffix}";
        }

        /// <summary>
        /// allows calling `ToString` with a format on a nullable float, ex: `price.ToString("c")`
        /// returns string.Empty is input is null.
        /// </summary>
        /// <param name="input">target nullable float</param>
        /// <param name="format">format for ToString if input is not null</param>
        /// <returns>formatted string</returns>
        public static string ToString(this float? input, string format)
        {
            return (input.HasValue) ? input.Value.ToString(format) : string.Empty;
        }

        /// <summary>
        /// allows calling `ToString` with a format on a nullable decimal, ex: `price.ToString("c")`
        /// returns string.Empty is input is null.
        /// </summary>
        /// <param name="input">target nullable decimal</param>
        /// <param name="format">format for ToString if input is not null</param>
        /// <returns>formatted string</returns>
        public static string ToString(this decimal? input, string format)
        {
            return (input.HasValue) ? input.Value.ToString(format) : string.Empty;
        }
        
        /// <summary>
        /// allows calling `ToString` with a format on a nullable double, ex: `price.ToString("c")`
        /// returns string.Empty is input is null.
        /// </summary>
        /// <param name="input">target nullable double</param>
        /// <param name="format">format for ToString if input is not null</param>
        /// <returns>formatted string</returns>
        public static string ToString(this double? input, string format)
        {
            return (input.HasValue) ? input.Value.ToString(format) : string.Empty;
        }

    }
}
