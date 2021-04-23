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

    }
}
