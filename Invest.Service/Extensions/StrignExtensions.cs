namespace Invest.Service.Extensions
{
    public static class StrignExtensions
    {
        public static string AsCnpj(this string cnpj)
        {
            return cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
        }

        public static string GetAssetType(this string code)
        {
            if (code.Contains("3"))
            {
                return "ON";
            }

            if (code.Contains("4"))
            {
                return "PN";
            }

            return "-";
        }
    }
}
