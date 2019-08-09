namespace VBaseProject.Entities.Domain
{
    public class Customer : BaseEntity
    {
        public int CustomerId { get; set; }
        public string Code { get; set; }
        public int StockExchangeId { get; set; }
        public int CompanyId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}
