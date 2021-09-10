using System;
using System.Collections.Generic;
using System.Linq;

namespace Essentials.Models
{
    public interface IPagedList<T>
    {
        T[] Items { get; set; }
        int TotalCount { get; }
        int Offset { get; }
        int PageSize { get; }

        int PageNumber { get; }
        int StartItem { get; }
        int EndItem { get; }
    }

    public class PagedList<T> : IPagedList<T>
    {
        public PagedList()
        {
            TotalCount = 0;
            Offset = 0;
            PageSize = 20;
        }

        public PagedList(IEnumerable<T> pageItems, int totalCount, int pageNumber = 1, int pageSize = 20)
        {
            Items = pageItems.ToArray();
            TotalCount = totalCount;

            if (pageNumber < 1)
                pageNumber = 1;
            Offset = (pageNumber - 1) * pageSize;

            if (pageSize < 1)
                pageSize = 1;
            PageSize = pageSize;
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
