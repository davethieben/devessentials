using System.Collections;

namespace Essentials
{
    public static class DictionaryCastExtensions
    {
        public static bool TryGetValue<TKey, TValue>(this IDictionary dictionary, TKey key, out TValue? value)
        {
            dictionary.IsRequired();
            if (dictionary.Contains(key))
            {
                object dictValue = dictionary[key];
                if (dictValue != default)
                {
                    if (typeof(TValue).IsAssignableFrom(dictValue.GetType()))
                    {
                        value = (TValue)dictValue;
                        return true;
                    }
                    else
                    {
                        throw new InvalidCastException($"Entry with key '{key}' was found but is of Type '{dictValue.GetType()}', not the requested '{typeof(TValue)}'");
                    }
                }
                else
                {
                    value = default;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public static IDictionary<TKey, TValue?> Cast<TKey, TValue>(this IDictionary? dictionary) =>
            new CastDictionary<TKey, TValue>(dictionary);

        private class CastDictionary<TKey, TValue> : IDictionary<TKey, TValue?>
        {
            private readonly IDictionary _inner;

            public CastDictionary(IDictionary? inner)
            {
                _inner = inner ?? new Hashtable();
            }

            public TValue? this[TKey key]
            {
                get
                {
                    if (_inner.TryGetValue<TKey, TValue>(key, out var value))
                        return value;
                    else
                        return default;
                }
                set => _inner[key] = value;
            }

            public ICollection<TKey> Keys => _inner.Keys.Cast<TKey>().ToList();

            public ICollection<TValue?> Values => _inner.Values.Cast<TValue?>().ToList();

            public int Count => _inner.Count;

            public bool IsReadOnly => _inner.IsReadOnly;

            public void Add(TKey key, TValue? value) => _inner.Add(key, value);

            public void Add(KeyValuePair<TKey, TValue?> item) => _inner.Add(item.Key, item.Value);

            public void Clear() => _inner.Clear();

            public bool Contains(KeyValuePair<TKey, TValue?> item)
            {
                if (ContainsKey(item.Key) && _inner.TryGetValue<TKey, TValue>(item.Key, out var value)
                    && value != null)
                {
                    return value.Equals(item.Value);
                }
                return false;
            }

            public bool ContainsKey(TKey key) => _inner.Contains(key);

            public void CopyTo(KeyValuePair<TKey, TValue?>[] array, int startIndex)
            {
                int currentIndex = startIndex;
                foreach (var pair in this)
                {
                    array[currentIndex] = pair;
                }
            }

            public bool Remove(TKey key)
            {
                if (_inner.Contains(key))
                {
                    _inner.Remove(key);
                    return true;
                }
                return false;
            }

            public bool Remove(KeyValuePair<TKey, TValue?> item) => Remove(item.Key);

            public bool TryGetValue(TKey key, out TValue? value) =>
                _inner.TryGetValue<TKey, TValue>(key, out value);

            public IEnumerator<KeyValuePair<TKey, TValue?>> GetEnumerator() =>
                new KeyValuePairEnumerator<TKey, TValue?>(_inner.GetEnumerator());

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private class KeyValuePairEnumerator<K, V> : IEnumerator<KeyValuePair<K, V>>
            {
                private readonly IDictionaryEnumerator _inner;

                public KeyValuePairEnumerator(IDictionaryEnumerator inner) => _inner = inner;

                public KeyValuePair<K, V> Current => new KeyValuePair<K, V>((K)_inner.Entry.Key, (V)_inner.Entry.Value);

                object IEnumerator.Current => Current;

                public bool MoveNext() => _inner.MoveNext();

                public void Reset() => _inner.Reset();

                public void Dispose() { }

            }

        }


    }
}
