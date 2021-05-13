namespace Essentials
{
    public static class PrimitiveExtensions
    {
        public static bool ToBool(this bool? target, bool @default = false) => target.HasValue ? target.Value : @default;

    }
}
