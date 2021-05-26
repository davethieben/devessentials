using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Essentials.Helpers
{
    public class ConcurrentList<T> : IEnumerable<T>
    {
        private readonly ConcurrentDictionary<int, Entry<T>> _store = new ConcurrentDictionary<int, Entry<T>>();

        public bool TryAdd(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return _store.TryAdd(item.GetHashCode(), new Entry<T> { Value = item, Index = _store.Count });
        }

        public bool TryRemove(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return _store.TryRemove(item.GetHashCode(), out Entry<T> _);
        }

        public IEnumerator<T> GetEnumerator() => _store.Values.OrderBy(x => x.Index).Select(x => x.Value).ToList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _store.Values.OrderBy(x => x.Index).Select(x => x.Value).ToList().GetEnumerator();

        private class Entry<TT>
        {
            public TT Value { get; set; } = default!;
            public int Index { get; set; }
        }

    }
}
