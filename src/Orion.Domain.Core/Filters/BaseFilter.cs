namespace Orion.Domain.Core.Filters;

public abstract class BaseFilter
{
    public int Page { get; init; } = 1;
    public int Quantity { get; init; } = 10;
    public string Query { get; init; }
}
