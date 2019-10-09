using System;
using System.Globalization;

namespace VBaseProject.Resources.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal ToDecimal(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            return Convert.ToDecimal(value.Replace(",", "."), new CultureInfo("en-US"));
        }

        /// <summary>
        /// Return the percentage from value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal PercentFrom(this int value, decimal secondValue)
        {
            var result = decimal.Round(secondValue * 100 / value, 2);

            return result;
        }

        public static decimal PercentFrom(this decimal value, decimal secondValue)
        {
            var result = decimal.Round(secondValue * 100 / value, 2);

            return result;
        }
    }
}
