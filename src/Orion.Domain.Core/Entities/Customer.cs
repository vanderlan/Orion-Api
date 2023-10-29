namespace Orion.Domain.Core.Entities;

public class Customer : BaseEntity
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
}
