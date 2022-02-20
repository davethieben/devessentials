using System;
using System.Collections.Generic;
using System.Linq;

namespace Essentials
{
    /// <summary>
    /// Helper for simple templating of strings
    /// </summary>
    public class StringReplacement
    {
        public const string TokenStart = "{";
        public const string TokenEnd = "}";

        private readonly string _input;
        private readonly Dictionary<string, Func<object?>> _tokens = new Dictionary<string, Func<object?>>();

        public StringReplacement(string input, IDictionary<string, string>? tokens = null)
        {
            _input = input ?? string.Empty;

            foreach (var kvp in tokens.EmptyIfNull())
                Add(kvp.Key, kvp.Value);
        }

        public StringReplacement Add(string token, string? replacement)
        {
            Contract.Requires(token, nameof(token));

            Add(token, () => replacement);
            return this;
        }

        public StringReplacement Add(string token, object? replacement)
        {
            Contract.Requires(token, nameof(token));

            Add(token, () => replacement);
            return this;
        }

        public StringReplacement Add(string token, Func<object?> replacement)
        {
            Contract.Requires(token, nameof(token));
            replacement.IsRequired();

            if ((token.StartsWith(TokenStart) && !token.EndsWith(TokenEnd)) || (!token.StartsWith(TokenStart) && token.EndsWith(TokenEnd)))
                throw new ArgumentException($"Invalid token: '{token}'; Token must not have braces or start and end with braces '{{', '}}'");

            //if(!token.StartsWith("{"))
            //    token = $"{{{token}}}";

            _tokens.Add(token, replacement);
            return this;
        }

        public override string ToString()
        {
            string output = _input;
            foreach (string token in _tokens.Keys)
            {
                object? value = _tokens[token]();
                output = output.Replace($"{TokenStart}{token}{TokenEnd}", value != null ? value.ToString() : string.Empty);
            }

            return output;
        }

        public static implicit operator string(StringReplacement input) => input.ToString();
        public static implicit operator StringReplacement(string input) => new StringReplacement(input);

        public static bool HasTokens(string input) => !string.IsNullOrEmpty(input) && input.Contains(TokenStart) && input.Contains(TokenEnd);

    }
}
