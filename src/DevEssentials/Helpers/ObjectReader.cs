using System;
using System.Collections.Generic;
using System.Linq;

namespace Essentials.Helpers
{
    public class ObjectReader
    {
        public ObjectReader(object? source)
        {
            Source = source ?? new object();
            SourceType = Source.GetType();
        }

        public object Source { get; }

        public Type SourceType { get; }

        public string[] Properties => SourceType.GetProperties().Select(p => p.Name).ToArray();

        public object? GetPropertyValue(string propertyName)
        {
            var property = SourceType.GetProperty(propertyName);
            if (property == null)
                throw new InvalidOperationException($"Unknown property: {propertyName}");

            return property.GetValue(Source);
        }

        public void CopyToDictionary(IDictionary<string, object?> destination, bool includeNullValues = false)
        {
            destination.IsRequired();
            foreach (var property in SourceType.GetProperties())
            {
                object value = property.GetValue(Source);
                if (value != null || includeNullValues)
                    destination[property.Name] = value;
            }
        }

        public IDictionary<string, object?> ToDictionary(bool includeNullValues = false)
        {
            var dictionary = new Dictionary<string, object?>();
            CopyToDictionary(dictionary, includeNullValues);
            return dictionary;
        }

    }
}
