using System;
using System.Collections.Generic;
using System.Globalization;

namespace Essentials
{
    public static class CultureExtensions
    {
        public static CultureInfo GetRoot(this CultureInfo input)
        {
            var current = input;
            while (current.Parent != CultureInfo.InvariantCulture)
                current = current.Parent;

            return current;
        }

    }

    public class CultureComparer : IEqualityComparer<CultureInfo>
    {
        public static CultureComparer Instance { get; } = new CultureComparer();

        private CultureComparer() { }

        public bool Equals(CultureInfo x, CultureInfo y)
        {
            if (x == null && y == null)
                return true;

            else if (x != null && y != null)
                return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);

            else
                return false;
        }

        public int GetHashCode(CultureInfo obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
