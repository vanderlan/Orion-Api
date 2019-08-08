namespace Invest.Entities.Domain
{
    public class Asset : BaseEntity
    {
        public int AssetId { get; set; }
        public string Code { get; set; }
        public int StockExchangeId { get; set; }
        public int CompanyId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}
