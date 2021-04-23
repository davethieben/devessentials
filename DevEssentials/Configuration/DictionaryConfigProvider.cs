using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Essentials.Configuration
{
    public class DictionaryConfigProvider : ConfigurationProvider, IConfigurationSource
    {
        public DictionaryConfigProvider(IEnumerable<KeyValuePair<string, string?>> data)
        {
            if (!data.IsNullOrEmpty())
                Data.AddRange(data);
        }

        public DictionaryConfigProvider(IEnumerable<KeyValuePair<string, IList<string?>>> data)
        {
            if (!data.IsNullOrEmpty())
            {
                foreach (var pair in data)
                {
                    if (pair.Value?.Count() <= 1)
                    {
                        Data.Add(pair.Key, pair.Value?.First());
                    }
                    else
                    {
                        for (int index = 0; index < pair.Value?.Count; index++)
                        {
                            // MSFT config creates keys like: SomeKey:0:SomeValue, SomeKey:1:SomeValue

                            Data.Add($"{pair.Key}:{index}", pair.Value[index]);
                        }
                    }
                }
            }
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => this;

    }

    public static class DictionaryConfigProviderExtensions
    {
        public static IConfigurationBuilder AddDictionary(this IConfigurationBuilder builder, IEnumerable<KeyValuePair<string, string?>> data) =>
            builder.Add(new DictionaryConfigProvider(data));

        public static IConfigurationBuilder AddDictionary(this IConfigurationBuilder builder, IEnumerable<KeyValuePair<string, IList<string?>>> data) =>
            builder.Add(new DictionaryConfigProvider(data));

    }
}