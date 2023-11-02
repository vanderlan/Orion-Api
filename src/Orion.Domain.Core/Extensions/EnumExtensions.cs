using System.ComponentModel;
using System.Linq;

namespace Orion.Domain.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string Description<T>(this T source)
        {
            var fieldInfo = source.GetType().GetField(source.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Any())
                return attributes[0].Description;

            return string.Empty;
        }
    }
}
