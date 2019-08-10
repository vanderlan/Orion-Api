namespace VBaseProject.Entities.Domain
{
    public class Customer : BaseEntity
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
