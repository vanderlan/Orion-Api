namespace Orion.Entities.Domain
{
    public class Customer : BaseEntity
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
    }
}
