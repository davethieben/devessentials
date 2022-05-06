namespace Essentials.Models
{
    public interface IPagedList<T>
    {
        T[] Items { get; set; }
        int Offset { get; }
        int PageSize { get; }
        int TotalCount { get; }

        int PageNumber { get; }
        int NumPages { get; }
        int StartItem { get; }
        int EndItem { get; }

    }

    public class PagedList<T> : IPagedList<T>
    {
        public static int DefaultPageSize = 100;

        public PagedList(int? pageSize = null)
        {
            TotalCount = 0;
            Offset = 0;
            PageSize = pageSize ?? DefaultPageSize;
        }

        public PagedList(IEnumerable<T> pageItems, int totalCount, int pageNumber = 1, int? pageSize = null)
        {
            Items = pageItems.ToArray();
            TotalCount = totalCount;

            if (pageNumber < 1)
                pageNumber = 1;

            pageSize ??= DefaultPageSize;
            if (pageSize < 1)
                pageSize = 1;
            PageSize = pageSize.Value;
            Offset = (pageNumber - 1) * pageSize.Value;
        }

        public T[] Items { get; set; } = default!;

        public int TotalCount { get; }

        public int Offset { get; }

        public int PageSize { get; }

        public int NumPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public int PageNumber => (int)Math.Ceiling((double)Offset / PageSize);

        public int StartItem
        {
            get
            {
                int n = Offset + 1;
                if (n > TotalCount)
                    return TotalCount;
                return n;
            }
        }

        public int EndItem
        {
            get
            {
                int n = Offset + PageSize;
                if (n > TotalCount)
                    return TotalCount;
                return n;
            }
        }

    }
}
