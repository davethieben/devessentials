using System;
using System.Collections.Generic;

namespace Essentials
{
    /// <summary>
    /// Helper for simple templating of strings
    /// </summary>
    public class StringReplacement
    {
        private readonly string _input;
        private readonly IDictionary<string, Func<object?>> _tokens;

        public StringReplacement(string input, IDictionary<string, string>? tokens = null)
        {
            _input = input ?? string.Empty;

            _tokens = new Dictionary<string, Func<object?>>();
            foreach (var kvp in tokens.EmptyIfNull())
                Add(kvp.Key, kvp.Value);
        }

        public StringReplacement Add(string token, string? replacement)
        {
            Contracts.Require(token, nameof(token));

            Add(token, () => replacement);
            return this;
        }

        public StringReplacement Add(string token, object? replacement)
        {
            Contracts.Require(token, nameof(token));

            Add(token, () => replacement);
            return this;
        }

        public StringReplacement Add(string token, Func<object?> replacement)
        {
            Contracts.Require(token, nameof(token));
            replacement.IsRequired();

            if ((token.StartsWith("{") && !token.EndsWith("}")) || (!token.StartsWith("{") && token.EndsWith("}")))
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
                output = output.Replace(token, value != null ? value.ToString() : string.Empty);
            }

            return output;
        }

        public static implicit operator string(StringReplacement input) => input.ToString();
        public static implicit operator StringReplacement(string input) => new StringReplacement(input);

    }
}
