namespace Essentials
{
    public static class PrimitiveExtensions
    {
        public static bool SafeEquals(this object? x, object? y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            return x.Equals(y);
        }

        public static bool ToBool(this bool? target, bool @default = false) => target.HasValue ? target.Value : @default;

    }
}
