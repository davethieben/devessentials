using System.Linq;
using Essentials.Models;
using Xunit;

namespace Essentials.Test.Models
{
    public class PagedListTests
    {

        [Fact]
        public void HasCorrectPageNumbers()
        {
            var items = Enumerable.Range(1, 10);
            var list = new PagedList<int>(items, 10);
            Assert.Equal(1, list.NumPages);

            items = Enumerable.Range(1, 20);
            list = new PagedList<int>(items, 20, pageSize: 20);
            Assert.Equal(1, list.NumPages);

            list = new PagedList<int>(items, 21, pageSize: 20);
            Assert.Equal(2, list.NumPages);

            list = new PagedList<int>(items, 99, pageSize: 20);
            Assert.Equal(5, list.NumPages);
        }

        [Fact]
        public void HandlesIncorrectPage()
        {
            var items = Enumerable.Range(1, 20);
            var list = new PagedList<int>(items, 20, pageSize: 20, pageNumber: 2);
            Assert.Equal(1, list.NumPages);
        }

        [Fact]
        public void HasCorrectOffset()
        {
            var items = Enumerable.Range(1, 20);
            var list = new PagedList<int>(items, 20, pageSize: 20, pageNumber: 1);
            Assert.Equal(0, list.Offset);

            list = new PagedList<int>(items, 50, pageSize: 20, pageNumber: 2);
            Assert.Equal(20, list.Offset);

            list = new PagedList<int>(items, 50, pageSize: 25, pageNumber: 2);
            Assert.Equal(25, list.Offset);
        }

        [Fact]
        public void HasCorrectStartEndNumbers()
        {
            var items = Enumerable.Range(1, 20);
            var list = new PagedList<int>(items, 20, pageSize: 20, pageNumber: 1);
            Assert.Equal(1, list.StartItem);
            Assert.Equal(20, list.EndItem);

            list = new PagedList<int>(items, 50, pageSize: 20, pageNumber: 2);
            Assert.Equal(21, list.StartItem);
            Assert.Equal(40, list.EndItem);

            list = new PagedList<int>(items, 50, pageSize: 25, pageNumber: 2);
            Assert.Equal(26, list.StartItem);
            Assert.Equal(50, list.EndItem);

            list = new PagedList<int>(items, 57, pageSize: 20, pageNumber: 3);
            Assert.Equal(41, list.StartItem);
            Assert.Equal(57, list.EndItem);

            list = new PagedList<int>(items, 41, pageSize: 20, pageNumber: 3);
            Assert.Equal(41, list.StartItem);
            Assert.Equal(41, list.EndItem);

            list = new PagedList<int>(items, 13, pageSize: 20, pageNumber: 1);
            Assert.Equal(1, list.StartItem);
            Assert.Equal(13, list.EndItem);

            list = new PagedList<int>(new int[0], 0, pageSize: 20, pageNumber: 1);
            Assert.Equal(0, list.StartItem);
            Assert.Equal(0, list.EndItem);

            list = new PagedList<int>(new int[] { 1 }, 1, pageSize: 20, pageNumber: 1);
            Assert.Equal(1, list.StartItem);
            Assert.Equal(1, list.EndItem);

        }


    }
}
