namespace Invest.Api.AutoMapper
{
    public class AssetInput
    {
        public string Code { get; set; }
        public int StockExchangeId { get; set; }
        public int CompanyId { get; set; }
        public int SectorId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int AssetId { get; set; }
        public string PublicId { get; internal set; }
    }
}
