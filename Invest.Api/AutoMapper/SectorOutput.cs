using System;

namespace Invest.Api.AutoMapper
{
    public class SectorOutput
    {
        public int SectorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StockExchangeId { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
