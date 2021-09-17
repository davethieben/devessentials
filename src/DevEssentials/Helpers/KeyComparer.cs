using System;
using System.Collections.Generic;

namespace Essentials
{
    public class KeyComparer<T, TResult> : IEqualityComparer<T>
        where T : class
        where TResult : IEquatable<TResult>
    {
        public KeyComparer(Func<T, TResult> keySelector)
        {
            Selector = keySelector.IsRequired();
        }

        public Func<T, TResult> Selector { get; set; }

        public bool Equals(T x, T y)
        {
            if ((x == null && y == null) || ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            var xKey = Selector(x);
            var yKey = Selector(y);

            if ((xKey == null && yKey == null) || ReferenceEquals(xKey, yKey))
                return true;

            return xKey?.Equals(yKey) == true;
        }

        public int GetHashCode(T obj)
        {
            obj.IsRequired();

            return Selector(obj)?.GetHashCode() ?? 0;
        }

    }
}
