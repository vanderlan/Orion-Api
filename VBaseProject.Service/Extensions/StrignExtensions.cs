using System;
using System.Security.Cryptography;
using System.Text;

namespace VBaseProject.Service.Extensions
{
    public static class StrignExtensions
    {
        #region SHA512
        public static string ToSHA512(this string text)
        {
            var crypt = new SHA512Managed();
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(text), 0, Encoding.ASCII.GetByteCount(text));
            var stringBuilder = new StringBuilder();

            foreach (byte theByte in crypto)
            {
                stringBuilder.Append(theByte.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
        #endregion
    }
}
