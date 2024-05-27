using System.Collections.Generic;

namespace Company.Orion.Domain.Core.ValueObjects.Pagination;

public class PagedList<T>(IList<T> items, int count = 0)
{
    public IList<T> Items { get; } = items;
    public int Count { get; } = count;
}
