using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Essentials.Helpers;

namespace Essentials
{
    public class QueryString
    {
        private readonly Dictionary<string, List<string?>> _values = new Dictionary<string, List<string?>>();
        private static readonly Func<string?, string> _encode = HttpUtility.UrlEncode;

        public QueryString(string? inputString = null)
        {
            if (!string.IsNullOrEmpty(inputString))
            {
                var pairs = HttpUtility.ParseQueryString(inputString).ToDictionary();
                foreach (var pair in pairs)
                {
                    _values.GetOrAdd(pair.Key)
                        .Add(pair.Value);
                }
            }
        }

        public QueryString(object? inputData, bool includeNullValues = false)
        {
            _values = new Dictionary<string, List<string?>>(
                    inputData.ToDictionary(includeNullValues)
                        .Select(kvp => new KeyValuePair<string, List<string?>>(
                            kvp.Key,
                            value: ToList(kvp.Value)))
                );

            List<string?> ToList(object? input)
            {
                if (input != null || includeNullValues)
                {
                    return new List<string?> { input?.ToString() };
                }
                return new List<string?>();
            }
        }

        public string? this[string key]
        {
            get => _values.GetOrAdd(key).ToDisplayString();
            set => Set(key, value);
        }

        public QueryString Add(string key, string? value)
        {
            _values.GetOrAdd(key).Add(value);
            return this;
        }

        public QueryString Set(string key, string? value)
        {
            var values = _values.GetOrAdd(key);
            values.Clear();
            values.Add(value);
            return this;
        }

        public QueryString Remove(string key)
        {
            _values.Remove(key);
            return this;
        }

        public override string ToString() => ToString(includeLeadingQuestion: false);

        public string ToString(bool includeLeadingQuestion = false)
        {
            return (includeLeadingQuestion ? "?" : "") +
                string.Join("&",
                    _values
                        .SelectMany(kvp => kvp.Value.Select(str => KeyValuePair.Create(kvp.Key, str)))
                        .Select(kvp => $"{_encode(kvp.Key)}={_encode(kvp.Value)}")
                );
        }

        public static string UpdateValue(string initialUrl, string key, string value)
        {
            var uri = new UriBuilder(initialUrl);
            var query = new QueryString(uri.Query);

            if (value == null)
            {
                query.Remove(key);
            }
            else
            {
                query.Set(key, value);
            }

            uri.Query = query.ToString();
            return uri.ToString();
        }

        public static string? GetQueryValue(string url, string key)
        {
            var uri = new UrlBuilder(url);
            var query = new QueryString(uri.Query);
            return query[key];
        }

    }
}
