using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Essentials
{
    public static class RegExExtensions
    {
        /// <summary>
        /// returns any captured group's values as strings (excluding the 0 index group)
        /// </summary>
        public static IEnumerable<string> GetGroupValues(this Match? match)
        {
            if (match?.Groups != null && match.Groups.Count > 1)
            {
                // skip the first Group because it always contains the entire Matched string
                for (int index = 1; index < match.Groups.Count; index++)
                    if (match.Groups[index].Value.HasValue())
                        yield return match.Groups[index].Value;
            }
        }

        public static string? GetGroupValue(this Match? match, int groupIndex = 1)
        {
            if (match?.Groups != null && match.Groups.Count > groupIndex)
                return match.Groups[groupIndex]?.Value;

            return null;
        }

        public static string? Match(this string input, string pattern) => 
            Regex.Match(input, pattern).GetGroupValue();

    }
}
