using System.Security.Cryptography;
using System.Text;

namespace Orion.Domain.Extensions
{
    public static class StrignExtensions
    {
        public static string ToSha512(this string text)
        {
            using SHA512 hashAlgorithm = SHA512.Create();

            var byteValue = Encoding.UTF8.GetBytes(text);
            var byteHash = hashAlgorithm.ComputeHash(byteValue);

            StringBuilder hex = new();

            foreach (var x in byteHash)
            {
                hex.Append(string.Format("{0:x2}", x));
            }

            return hex.ToString();
        }
    }
}
