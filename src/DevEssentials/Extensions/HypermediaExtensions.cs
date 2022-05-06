namespace Essentials.Models
{
    public static class HypermediaExtensions
    {
        public static IEnumerable<Hypermedia<T>> WrapHypermedia<T>(this IEnumerable<T> list) where T : class =>
            list.Select(item => new Hypermedia<T>(item));


    }
}
