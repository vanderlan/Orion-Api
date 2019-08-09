namespace VBaseProject.Service.Extensions
{
    public static class StrignExtensions
    {
        public static string AsCnpj(this string cnpj)
        {
            return cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
        }

        public static string GetCustomerType(this string code)
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
