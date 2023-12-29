using System.Collections.Generic;

namespace Orion.Domain.Core.ValueObjects.Pagination;

public class PagedList<T>(IList<T> items, int count = 0)
{
    public IList<T> Items { get; set; } = items;
    public int Count { get; set; } = count;
}
