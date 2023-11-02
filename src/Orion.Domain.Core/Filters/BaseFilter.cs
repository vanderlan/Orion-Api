namespace Orion.Domain.Core.Filters;

public abstract class BaseFilter
{
    public int Page { get; set; } = 1;
    public int Quantity { get; set; } = 10;
    public string Query { get; set; }
}
