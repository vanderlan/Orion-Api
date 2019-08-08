using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Invest.Api.AutoMapper
{
    public class AssetOutput
    {
        public string PublicId { get; set; }
        public string Code { get; set; }
        public int StockExchangeId { get; set; }
        public int CompanyId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public virtual List<FundamentalistIndicatorsOutput> FundamentalistIndicatorsList { get; set; }
        public FundamentalistIndicatorsOutput LastFundamentalistIndicators { get; set; }
    }
}
