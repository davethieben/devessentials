namespace Essentials.Models
{
    public static class PagedListExtensions
    {
        public static int? NextPage<T>(this IPagedList<T> page)
        {
            page.IsRequired();
            if (page.EndItem < page.TotalCount)
                return page.PageNumber + 1;
            return null;
        }

        public static int? PreviousPage<T>(this IPagedList<T> page)
        {
            page.IsRequired();
            if (page.StartItem > 0)
                return page.PageNumber - 1;
            return null;
        }

        public static int? NextPageOffset<T>(this IPagedList<T> page)
        {
            page.IsRequired();
            if (page.EndItem < page.TotalCount)
                return page.Offset + page.PageSize;
            return null;
        }

        public static int? PreviousPageOffset<T>(this IPagedList<T> page)
        {
            page.IsRequired();
            if (page.StartItem > 0)
                return page.Offset - page.PageSize;
            return null;
        }

        public static IPagedList<T> Select<S, T>(this IPagedList<S> list, Func<S, T> transform)
        {
            list.IsRequired();
            return new PagedList<T>(list.Items.Select(transform), list.TotalCount, list.PageNumber, list.PageSize);
        }


    }
}
