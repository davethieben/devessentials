using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Essentials
{
    public static class ConfigurationExtensions
    {
        public static string GetJson(this IConfiguration config, Formatting formatting = Formatting.Indented, params JsonConverter[] converters)
        {
            JToken Visit(IConfiguration current)
            {
                var output = new JObject();
                var currentSection = current as IConfigurationSection;

                if (currentSection != null && currentSection.Value.HasValue())
                {
                    output[currentSection.Key] = currentSection.Value;
                }
                else
                {
                    var sections = current.GetChildren();
                    if (!sections.IsNullOrEmpty())
                    {
                        if (currentSection != null && sections.First().Key == "0") // possibly an array
                        {
                            var indexes = sections.Select(s => s.Key.ToNullableInt())
                                .Where(i => i != null)
                                .Select(i => i!.Value);

                            if (indexes.IsEqualTo(Enumerable.Range(0, sections.Count())))
                            {
                                var array = new JArray();

                                foreach (var section in sections)
                                {
                                    if (section.Value.HasValue())
                                    {
                                        array.Add(section.Value);
                                    }
                                    else
                                    {
                                        array.Add(Visit(section));
                                    }
                                }

                                return array;
                            }
                        }

                        foreach (var section in sections)
                        {
                            if (section.Key.HasValue() && section.Value.HasValue())
                            {
                                output[section.Key] = section.Value;
                            }
                            else
                            {
                                output[section.Key] = Visit(section);
                            }
                        }

                    }
                }

                return output;
            }

            JToken final = Visit(config);

            return final.ToString(formatting, converters);
        }

        public static IDictionary<string, string> GetData(this IConfiguration config)
        {
            void Visit(Dictionary<string, string> output, string? parentName, IConfiguration current)
            {
                if (current == null)
                    return;

                var currentSection = current as IConfigurationSection;

                if (parentName.HasValue())
                    parentName += ":";

                if (currentSection != null && currentSection.Value.HasValue())
                {
                    output[currentSection.Key] = currentSection.Value;
                }
                else
                {
                    var sections = current.GetChildren();
                    if (!sections.IsNullOrEmpty())
                    {
                        foreach (var section in sections)
                        {
                            if (section.Value.HasValue())
                            {
                                output[parentName + section.Key] = section.Value;
                            }
                            else
                            {
                                Visit(output, parentName + section.Key, section);
                            }
                        }
                    }
                }
            }

            var data = new Dictionary<string, string>();

            Visit(data, null, config);

            return data;
        }

    }
}