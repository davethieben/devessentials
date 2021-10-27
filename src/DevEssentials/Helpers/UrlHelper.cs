using System;
using System.Diagnostics.CodeAnalysis;

namespace Essentials
{
    public static class UrlHelper
    {
        /// <summary>
        /// tests if a string URL is safe for local redirect
        /// </summary>
        public static bool IsRelative(
#if NETSTANDARD2_1_OR_GREATER
            [NotNullWhen(true)] 
#endif
            string? input)
        {
            if (!string.IsNullOrEmpty(input)
                && Uri.TryCreate(input, UriKind.RelativeOrAbsolute, out Uri result)
                && !result.IsAbsoluteUri)
                return true;

            return false;
        }

#if NETSTANDARD2_1_OR_GREATER
        [return: NotNull]
#endif
        public static string GetSafeUrl(string? input, string? @default = "/")
        {
            if (input != null && IsRelative(input))
                return input;
            else
                return @default ?? string.Empty;
        }

    }
}
