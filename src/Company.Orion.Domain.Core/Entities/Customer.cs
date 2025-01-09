namespace Company.Orion.Domain.Core.Entities
{
    public class Customer : BaseEntity
    {
        public long CustomerId { get; set; }
        public required string Name { get; set; }
    }
}
