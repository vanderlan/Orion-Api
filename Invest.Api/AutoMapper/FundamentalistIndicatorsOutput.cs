using System;

namespace Invest.Api.AutoMapper
{
    public class FundamentalistIndicatorsOutput
    {
        public string PublicId { get; set; }
        public AssetOutput Asset { get; set; }


        public decimal? ROE { get; set; }
        public decimal? PL { get; set; }
        public decimal? LPA { get; set; }
        public decimal? MargBruta { get; set; }
        public decimal? MargEBIT { get; set; }
        public decimal? MargLiquida { get; set; }
        public decimal? EBITporAtivo { get; set; }
        public decimal? ROIC { get; set; }
        public decimal? LiquidezCorr { get; set; }
        public decimal? DivBrporPatrim { get; set; }
        public DateTime Date { get; set; }
        public decimal? DividendYield { get; set; }
    }
}
