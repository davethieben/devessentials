using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace Essentials
{
    public static class StringExtensions
    {
        public static bool HasValue(
#if NETSTANDARD2_1
            [NotNullWhen(true)] 
#endif
            this string? target)
        {
            return !string.IsNullOrEmpty(target);
        }

        public static bool IsNullOrEmpty(
#if NETSTANDARD2_1
            [MaybeNullWhen(true)] 
#endif
            this string? target)
        {
            return string.IsNullOrEmpty(target);
        }

        public static bool IsNullOrWhiteSpace(
#if NETSTANDARD2_1
            [MaybeNullWhen(true)] 
#endif
            this string? target)
        {
            return string.IsNullOrWhiteSpace(target);
        }

        /// <summary>
        /// similar to string.Equals but case insensitive
        /// </summary>
        public static bool IsEquivalent(this string? x, string? y) =>
            string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// similar to string.Contains but case insensitive
        /// </summary>
        public static bool ContainsEquivalent(this string x, string y) =>
            !string.IsNullOrEmpty(x) && x.IndexOf(y, StringComparison.OrdinalIgnoreCase) >= 0;

        public static int ToInt(this string? target, int @default = 0) =>
            Int32.TryParse(target, out int attempt) ? attempt : @default;

        public static int? ToInt(this StringValues strings) => ((string)strings).ToInt();

        public static long ToLong(this string? target, long @default = 0) =>
            Int64.TryParse(target, out long attempt) ? attempt : @default;

        public static int? ToNullableInt(this string? target) =>
            Int32.TryParse(target, out int attempt) ? attempt : (int?)null;

        public static bool ToBool(this string? target, bool @default = false)
        {
            if (Boolean.TryParse(target, out bool attempt))
                return attempt;

            int? attempt2 = target.ToNullableInt();
            if (attempt2.HasValue)
                return attempt2.Value != 0;

            if (target.IsEquivalent("yes") || target.IsEquivalent("on"))
                return true;

            if (target.IsEquivalent("no") || target.IsEquivalent("off"))
                return false;

            return @default;
        }

#if NETSTANDARD2_1
        [return: NotNull]
#endif
        public static string SubstringAfter(this string? input, string target)
        {
            if (input == null || string.IsNullOrEmpty(input))
                return string.Empty;

            Contract.Requires(target, nameof(target));

            int index = input.IndexOf(target);
            if (index < 0)
                return string.Empty;

            return input.Substring(index + target.Length);
        }

#if NETSTANDARD2_1
        [return: NotNull]
#endif
        public static string SubstringBefore(this string? input, string target)
        {
            if (input == null || string.IsNullOrEmpty(input))
                return string.Empty;

            Contract.Requires(target, nameof(target));

            int index = input.IndexOf(target);
            if (index < 0)
                index = input.Length;

            return input.Substring(0, index);
        }

#if NETSTANDARD2_1
        [return: NotNull]
#endif
        public static string SafeSubstring(this string? input, int desiredLength)
        {
            if (input == null || string.IsNullOrEmpty(input))
                return string.Empty;
            else if (input.Length > desiredLength)
                return input.Substring(0, desiredLength);
            else
                return input;
        }

#if NETSTANDARD2_1
        [return: NotNull]
#endif
        public static string Left(this string? input, int desiredLength) => SafeSubstring(input, desiredLength);

#if NETSTANDARD2_1
        [return: NotNull]
#endif
        public static string Right(this string? input, int desiredLength)
        {
            if (input == null || string.IsNullOrEmpty(input))
                return string.Empty;
            else if (input.Length > desiredLength)
            {
                int start = input.Length - desiredLength;
                return input.Substring(start, desiredLength);
            }
            else
                return input;
        }

        public static string? NullSafeReplace(this string? input, string oldValue, string newValue)
        {
            if (input != null && !string.IsNullOrEmpty(input))
                input = input.Replace(oldValue, newValue);

            return input;
        }

        public static string? RemoveAll(this string? input, params char[] targets)
        {
            if (input == null || string.IsNullOrEmpty(input))
                return input;

            string output = input;
            for (int i = 0; i < targets.Length; i++)
                output = output.Replace(targets[i].ToString(), string.Empty);

            return output;
        }

        public static bool ContainsIgnoreCase(this string? input, string test)
        {
            Contract.Requires(test, nameof(test));

            if (input == null)
                return false;

            return input.IndexOf(test, StringComparison.OrdinalIgnoreCase) >= 0;
        }

#if NETSTANDARD2_1
        [return: NotNull]
#endif
        public static string ToBase64Encoded(this string? text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text ?? string.Empty));
        }

#if NETSTANDARD2_1
        [return: NotNull]
#endif
        public static string FromBase64Encoded(this string? encoded)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encoded ?? string.Empty));
        }

#if NETSTANDARD2_1
        [return: NotNull]
#endif
        public static string ToUrlSafe(this string? input)
        {
            if (input == null)
                return string.Empty;

            var output = Regex.Replace(input.ToLower(), "[^a-z0-9]+", "-");

            return output;
        }


    }

    public static class Strings
    {
        /// <summary>
        /// converts kebab case ("words-separated-by-skewers") to Pascal case ("WordsSeparatedByCapitals")
        /// </summary>
        public static string KebabToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var parts = input.ToLower().Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            var output = new StringBuilder();

            for (int i = 0; i < parts.Length; i++)
            {
                output.Append(parts[i][0].ToString().ToUpper());
                output.Append(parts[i].Skip(1).ToArray());
            }

            return output.ToString();
        }

        /// <summary>
        /// converts a URL-like string into a namespace-like string. useful for creating logging categories per URL
        /// </summary>
        /// <example>
        ///     in: /path/to/some/location
        ///    out: Path.To.Some.Location
        /// </example>
        public static string UrlToNamespace(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var parts = input.ToLower().Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var output = new StringBuilder();

            for (int i = 0; i < parts.Length; i++)
            {
                output.Append(parts[i][0].ToString().ToUpper());
                output.Append(parts[i].Skip(1).ToArray());

                if (i < (parts.Length - 1))
                    output.Append(".");
            }

            return output.ToString();
        }

        public static string ToCamelCase(this string input)
        {
            char[] output = input.ToCharArray();

            for (int i = 0; i < output.Length; i++)
            {
                if (char.IsUpper(output[i]))
                {
                    output[i] = char.ToLower(output[i]);
                }
                else
                {
                    break;
                }
            }

            return new string(output);
        }

        /// <summary>
        /// converts a string of any characters into a URL-suitable token
        /// </summary>
        /// <example>
        ///     in: This 2 Could? Be Any^thing!!
        ///    out: this-2-could-be-anything
        /// </example>
        public static string ToUrlKey(this string? input)
        {
            input = (input ?? "").ToLower();

            return _urlKeyRemove.Replace(input, string.Empty);
        }
        private static Regex _urlKeyRemove = new Regex(@"[^a-zA-Z0-9\-]+", RegexOptions.Compiled);

        /// <summary>
        /// converts arbitrary strings of parts of a URL into one, well-formed URL
        /// </summary>
        public static string CombineUri(params string[] uriParts)
        {
            string uri = string.Empty;

            if (!uriParts.IsNullOrEmpty())
            {
                char[] trims = new char[] { '\\', '/' };

                uri = (uriParts[0] ?? string.Empty).TrimEnd(trims);

                for (int index = 1; index < uriParts.Count(); index++)
                    uri = string.Format($"{uri.TrimEnd(trims)}/{(uriParts[index] ?? string.Empty).TrimStart(trims)}");
            }

            return uri;
        }

        /// <summary>
        /// creates a non-unique random string of characters suitable for IDs
        /// </summary>
        /// <param name="length">desired length of the output string, between 1 and 255</param>
        /// <returns></returns>
        public static string GenerateId(int length = 8)
        {
            if (length < 1 || length > 255)
                throw new ArgumentOutOfRangeException(nameof(length), "get real");

            var output = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                string sample = string.Empty;
                int position = 0;

                while (output.Length < length)
                {
                    if (position >= sample.Length)
                    {
                        position = 0;

                        var buffer = new byte[length];
                        rng.GetBytes(buffer);

                        sample = Convert.ToBase64String(buffer).RemoveAll('+', '=', '/') ?? string.Empty;
                    }

                    output.Append(sample[position++]);
                }
            }

            return output.ToString();
        }

        public static IDictionary<string, string?> JsonToDictionary(string? input)
        {
            var output = new Dictionary<string, string?>();
            if (input.HasValue())
            {
                var jObject = JObject.Parse(input);
                foreach (KeyValuePair<string, JToken?> kvp in jObject)
                {
                    output[kvp.Key] = kvp.Value?.ToString();
                }
            }

            return output;
        }

        /// <summary>
        /// similar to string.Join but doesn't include null or whitespace-only strings
        /// </summary>
        public static string? JoinNotEmpty(string separator, IEnumerable<string>? values) =>
            values != null ? string.Join(separator, values.Where(s => !string.IsNullOrWhiteSpace(s))) : null;

        public static string? JoinNotEmpty(string separator, params string[]? values) =>
            JoinNotEmpty(separator, values.AsEnumerable());

    }
}
