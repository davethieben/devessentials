using System;

namespace Essentials
{
    public static class UrlHelper
    {
        /// <summary>
        /// tests is a string URL is safe for local redirect
        /// </summary>
        public static bool IsRelative(string input)
        {
            if (!string.IsNullOrEmpty(input)
                && Uri.TryCreate(input, UriKind.RelativeOrAbsolute, out Uri result)
                && !result.IsAbsoluteUri)
                return true;

            return false;
        }

    }
}
