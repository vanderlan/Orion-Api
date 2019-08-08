namespace Invest.Entities.ValueObjects.Analysis
{
    public class AssetsAnalysisValueObject
    {
        public string Publicid { get; set; }
        public string Company { get; set; }
        public string Code { get; set; }
        private decimal Variation { get; set; }
        public decimal VariationPercentage
        {
            get
            {
                return decimal.Round(Variation, 2);
            }
        }

        public decimal Start { get; set; }
        public decimal Now { get; set; }

        /// <summary>
        /// Fundamentus Details
        /// </summary>
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
        public decimal? DividendYield { get; set; }
    }
}
