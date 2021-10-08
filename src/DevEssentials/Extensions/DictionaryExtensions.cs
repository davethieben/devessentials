using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace Essentials
{
    public static class DictionaryExtensions
    {

#if NETSTANDARD2_1_OR_GREATER
        [return: NotNull]
#endif
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TKey : notnull
            where TValue : new()
        {
            dictionary.IsRequired();

            if (!dictionary.TryGetValue(key, out TValue value))
            {
                value = new TValue();
                dictionary[key] = value;
            }

            return value.IsRequired();
        }

        public static TValue? GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue?> dictionary, TKey key, Func<TValue?> factory)
        {
            dictionary.IsRequired();

            if (!dictionary.TryGetValue(key, out TValue? value))
            {
                value = factory.IsRequired().Invoke();
                dictionary[key] = value;
            }

            return value;
        }

        public static IDictionary<string, object?> ToDictionary(this object? input, bool includeNullValues = true)
        {
            var output = new Dictionary<string, object?>();

            if (input != null)
            {
                Type inputType = input.GetType().IsRequired();
                var properties = inputType.GetProperties(System.Reflection.BindingFlags.Instance);
                foreach (PropertyInfo property in properties)
                {
                    object? value = property.GetValue(input);
                    if (value != null || includeNullValues)
                    {
                        output.Add(property.Name, value);
                    }
                }
            }

            return output;
        }

#if NETSTANDARD2_1_OR_GREATER
        [return: MaybeNull]
#endif
        public static TValue? TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TKey : notnull
        {
            TValue? value = default;
            dictionary?.TryGetValue(key, out value);
            return value;
        }

        public static async Task<TValue> GetOrAddAsync<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<Task<TValue>> factory)
            where TKey : notnull
        {
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                value = await factory.IsRequired().Invoke();
                dictionary[key] = value;
            }

            return value;
        }

        /// <summary>
        /// attempt to retrieve and/or add a value to dictionary that may be shared with other resources.
        /// ConcurrentDictionary supports key locking, but doesn't lock the entire dictionary.
        /// </summary>
        public static TValue GetOrAddLocked<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
            where TKey : notnull
        {
            dictionary.IsRequired();

            if (!dictionary.TryGetValue(key, out TValue value))
            {
                lock (dictionary)
                {
                    if (!dictionary.TryGetValue(key, out value))
                    {
                        value = factory.IsRequired().Invoke(key);
                        dictionary.Add(key, value);
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// returns a new Dictionary[[TKey, TValue]] if the input is null
        /// </summary>
#if NETSTANDARD2_1_OR_GREATER
        [return: NotNull]
#endif
        public static IDictionary<TKey, TValue> EmptyIfNull<TKey, TValue>(this IDictionary<TKey, TValue>? dictionary)
            where TKey : notnull
        {
            return dictionary ?? new Dictionary<TKey, TValue>();
        }

        public static IDictionary<string, TValue> IgnoreKeyCase<TValue>(this IDictionary<string, TValue>? dictionary)
        {
            if (dictionary != null)
                return new Dictionary<string, TValue>(dictionary, StringComparer.OrdinalIgnoreCase);
            else
                return new Dictionary<string, TValue>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// retrieves an item from the dictionary with the key of typeof(T).FullName
        /// </summary>
#if NETSTANDARD2_1_OR_GREATER
        [return: MaybeNull]
#endif
        public static T? Get<T>(this IDictionary<string, object> dictionary)
        {
            dictionary.IsRequired();
            string key = Contract.Requires(typeof(T).FullName, $"{typeof(T)} FullName");

            return dictionary.TryGetValue(key, out object? attempt) && attempt != null ? (T)attempt : default;
        }

#if NETSTANDARD2_1_OR_GREATER
        [return: MaybeNull]
#endif
        public static T? Get<T>(this IDictionary<object, object> dictionary)
        {
            dictionary.IsRequired();
            string key = Contract.Requires(typeof(T).FullName, $"{typeof(T)} FullName");

            return dictionary.TryGetValue(key, out object? attempt) && attempt != null ? (T)attempt : default;
        }

        /// <summary>
        /// sets an item in the dictionary with the key of typeof(T).FullName
        /// </summary>
        public static void Set<T>(this IDictionary<string, object?> dictionary, T item)
        {
            dictionary.IsRequired();
            string key = Contract.Requires(typeof(T).FullName, $"{typeof(T)} FullName");

            dictionary[key] = item;
        }

        public static void Set<T>(this IDictionary<object, object?> dictionary, T item)
        {
            dictionary.IsRequired();
            string key = Contract.Requires(typeof(T).FullName, $"{typeof(T)} FullName");

            dictionary[key] = item;
        }

        /// <summary>
        /// sets an item in the dictionary with the key of typeof(T).FullName
        /// </summary>
        public static void Set(this IDictionary<string, object> dictionary, object item)
        {
            dictionary.IsRequired();

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            string key = Contract.Requires(item.GetType().FullName, $"{item.GetType()} FullName");
            dictionary[key] = item;
        }

        public static IDictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue>? source)
            where TKey : notnull
        {
            var destination = new Dictionary<TKey, TValue>();
            foreach (KeyValuePair<TKey, TValue> item in source.EmptyIfNull())
                destination.Add(item.Key, item.Value);

            return destination;
        }

        /// <summary>
        /// appends the elements of the secondary dictionary to the primary
        /// </summary>
        /// <param name="primary">Dictionary to extend with elements from the secondary</param>
        /// <param name="secondary">elements to add to the primary</param>
        /// <param name="overwrite">if true, matching elements in the secondary will overwrite those in the primary. if false, they are skipped.</param>
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> primary, IEnumerable<KeyValuePair<TKey, TValue>> secondary, bool overwrite = false)
            where TKey : notnull
        {
            if (primary == null || secondary == null)
                return;

            foreach (var item in secondary)
            {
                if (primary.ContainsKey(item.Key))
                {
                    if (overwrite)
                        primary[item.Key] = item.Value;
                }
                else
                {
                    primary.Add(item);
                }
            }
        }

        public static void AddValue<TKey, TValue>(this IDictionary<TKey, IList<TValue>> dictionary, TKey key, TValue value)
            where TKey : notnull
            => dictionary.IsRequired().AddValues(key, new[] { value });

        public static void AddValues<TKey, TValue>(this IDictionary<TKey, IList<TValue>> dictionary, TKey key, IEnumerable<TValue> values)
            where TKey : notnull
        {
            dictionary.IsRequired();

            if (!dictionary.TryGetValue(key, out IList<TValue>? sublist))
            {
                sublist = new List<TValue>();
                dictionary.Add(key, sublist);
            }

            sublist.AddRange(values);
        }

    }
}
