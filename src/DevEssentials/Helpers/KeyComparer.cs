using System;
using System.Collections.Generic;

namespace Essentials
{
    public class KeyComparer<T> : IEqualityComparer<T>
        where T : class
    {
        public KeyComparer(Func<T, object> keySelector)
        {
            Selector = keySelector.IsRequired();
        }

        //public KeyComparer(Expression<Func<T, object>> keySelector)
        //{
        //    Selector = keySelector.IsRequired().Compile();
        //}

        public Func<T, object> Selector { get; set; }

        public bool Equals(T x, T y)
        {
            if (x == null && y == null)
                return true;

            if (x != null && y != null)
            {
                var xKey = Selector(x);
                var yKey = Selector(y);

                if (xKey == null && yKey == null)
                    return true;

                if (xKey != null && yKey != null)
                {
                    return xKey.Equals(yKey);
                }
            }

            return false;
        }

        public int GetHashCode(T obj)
        {
            obj.IsRequired();

            return Selector(obj)?.GetHashCode() ?? 0;
        }

    }
}
