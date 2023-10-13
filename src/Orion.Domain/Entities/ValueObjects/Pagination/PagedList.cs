using System.Collections.Generic;

namespace Orion.Domain.Entities.ValueObjects.Pagination
{
    public class PagedList<T>
    {
        public PagedList(IList<T> items, int count = 0)
        {
            Items = items;
            Count = count;
        }

        public IList<T> Items { get; set; }
        public int Count { get; set; }
    }
}
