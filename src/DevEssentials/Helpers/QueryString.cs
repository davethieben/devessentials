using System.Net;

namespace Essentials
{
    public class QueryString
    {
        private readonly Dictionary<string, List<string>> _values = new Dictionary<string, List<string>>();
        private static readonly Func<string, string> _encode = WebUtility.UrlEncode;

        public QueryString(string? inputString = null)
        {
            if (inputString != null && !string.IsNullOrEmpty(inputString))
            {
                if (inputString.Contains("?"))
                    inputString = inputString.SubstringAfter("?");

                string[] pairs = inputString.Split('&');
                foreach (var pair in pairs)
                {
                    string key;
                    string? value;

                    if (!pair.Contains("="))
                    {
                        key = pair;
                        value = null;
                    }
                    else
                    {
                        key = pair.SubstringBefore("=");
                        value = pair.SubstringAfter("=");
                    }

                    var values = _values.GetOrAdd(key);
                    if (value != null)
                        values.Add(value);
                }
            }
        }

        public QueryString(object? inputData, bool includeNullValues = false)
        {
            _values.AddRange(
                inputData.ToDictionary(includeNullValues)
                    .Select(kvp => new KeyValuePair<string, List<string>>(kvp.Key, value: ToList(kvp.Value)))
                );

            List<string> ToList(object? input)
            {
                if (input != null || includeNullValues)
                {
                    return new List<string> { input?.ToString() ?? "" };
                }
                return new List<string>();
            }
        }

        public string? this[string key]
        {
            get => _values.GetOrAdd(key).ToDisplayString();
            set => Set(key, value);
        }

        public QueryString Add(string key, string? value)
        {
            var values = _values.GetOrAdd(key);
            if (value != null)
                values.Add(value);

            return this;
        }

        public QueryString Set(string key, string? value)
        {
            var values = _values.GetOrAdd(key);
            values.Clear();

            if (value != null)
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
                        .SelectMany(kvp => kvp.Value.Select(str => new KeyValuePair<string, string>(kvp.Key, str)))
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
