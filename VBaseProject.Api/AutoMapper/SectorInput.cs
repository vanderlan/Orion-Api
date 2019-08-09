namespace VBaseProject.Api.AutoMapper
{
    public class SectorInput
    {
        public int SectorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StockExchangeId { get; set; }
        public string PublicId { get; internal set; }
    }
}
