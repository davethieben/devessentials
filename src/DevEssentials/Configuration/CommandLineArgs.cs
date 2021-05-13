using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Essentials.Configuration
{
    /// <summary>
    /// this CLI args class differs from the .NET Core configuration provider (.AddCommandLine(args)) in that it allows arguments with no value,
    /// such as: ONeSOURCEClient.exe -browse -product a|b|c
    /// </summary>
    public class CommandLineArgs : Dictionary<string, IList<string?>>, IConfigurationSource
    {
        public CommandLineArgs() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public static CommandLineArgs Parse(IEnumerable<string> input)
        {
            var data = new CommandLineArgs();
            string? key = null, value = null;

            if (input != null)
            {
                using (var enumerator = input.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        string currentArg = enumerator.Current;
                        string? currentKey = null;

                        if (currentArg.StartsWith("-"))
                        {
                            currentKey = currentArg.TrimStart('-');

                            if (key.HasValue() && currentKey.HasValue())
                            {
                                data.Add(key, null);
                            }

                            key = currentKey;
                        }
                        else
                        {
                            value = currentArg;
                        }

                        if (key.HasValue() && value.HasValue())
                        {
                            data.Add(key, value);

                            key = null;
                            value = null;
                        }

                    }
                }

                if (key.HasValue())
                    data.Add(key, null);

            }

            return data;
        }

        private static IList<R> TryGetList<T, R>(IDictionary<T, IList<R>> dictionary, T key)
            where T : notnull
        {
            if (!dictionary.TryGetValue(key, out IList<R>? list) || list == null)
            {
                list = new List<R>();
                dictionary[key] = list;
            }

            return list;
        }

        public void Add(string key, string? value)
        {
            var values = TryGetList(this, key);
            values.Add(value);
        }

        public void AddIfValue(string key, string? value)
        {
            if (value.HasValue())
                Add(key, value);
        }

        /// <summary>
        /// generates a string of key/value pairs in a URL format querystring. not URL encoded.
        /// </summary>
        /// <returns></returns>
        public string SerializeToQueryString()
        {
            var queryString = new QueryString();

            foreach (var pair in this)
            {
                if (pair.Value != null && pair.Value.Count > 0)
                {
                    foreach (string? item in pair.Value)
                    {
                        queryString.Add(pair.Key, item ?? "");
                    }
                }
                else
                {
                    queryString.Add(pair.Key, "");
                }
            }

            return queryString.ToString();
        }

        /// <summary>
        /// generates a string of key/value pairs in the format that MSFT CommandLine config provider expects
        /// </summary>
        /// <param name="argPrefix">string to prefix to all keys. defaults to MSFT convention "--"</param>
        /// <returns></returns>
        public string SerializeToCommandLine(string argPrefix = "--")
        {
            var output = new StringBuilder();

            foreach (var pair in this)
            {
                if (pair.Value != null && pair.Value.Count > 0)
                {
                    foreach (string? item in pair.Value)
                    {
                        output.Append($"{argPrefix}{pair.Key} {item ?? ""} ");
                    }
                }
            }

            return output.ToString();
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DictionaryConfigProvider(this);
        }

    }
}
