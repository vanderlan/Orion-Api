namespace Orion.Core.Domain.Entities;

public class Customer : BaseEntity
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
}
