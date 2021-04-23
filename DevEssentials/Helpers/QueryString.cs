using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Essentials
{
    public class QueryString
    {
        private readonly Dictionary<string, List<string>> _values = new Dictionary<string, List<string>>();
        private static readonly Func<string, string> _encode = System.Web.HttpUtility.UrlEncode;

        public QueryString(string? inputString = null)
        {
            if (!string.IsNullOrEmpty(inputString))
            {
                throw new NotImplementedException();
            }
        }

        public QueryString(object? inputData)
        {
            _values = new Dictionary<string, List<string>>(
                    inputData.ToDictionary(includeNullValues: false)
                        .Select(kvp => new KeyValuePair<string, List<string>>(
                            kvp.Key,
                            value: kvp.Value != null ? new List<string> { kvp.Value.ToString() } : new List<string>()))
                );
        }

        public QueryString Add(string key, string value)
        {
            _values.GetOrAdd(key).Add(value);
            return this;
        }

        public override string ToString()
        {
            return string.Join("&",
                _values
                    .SelectMany(kvp => kvp.Value.Select(str => KeyValuePair.Create(kvp.Key, str)))
                    .Select(kvp => $"{_encode(kvp.Key)}={_encode(kvp.Value)}")
            );
        }


    }
}
