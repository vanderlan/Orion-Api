using System.Security.Cryptography;
using System.Text;

namespace Orion.Domain.Extensions
{
    public static class StrignExtensions
    {
        public static string ToSha512(this string text)
        {
            var byteValue = Encoding.UTF8.GetBytes(text);
            var byteHash = SHA512.HashData(byteValue);

            StringBuilder hex = new();

            foreach (var x in byteHash)
            {
                hex.Append(string.Format("{0:x2}", x));
            }

            return hex.ToString();
        }
    }
}
