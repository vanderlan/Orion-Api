using System.ComponentModel;

namespace Company.Orion.Domain.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string Description<T>(this T source)
        {
            var fieldInfo = source.GetType().GetField(source.ToString() ?? string.Empty);

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length != 0)
                return attributes[0].Description;

            return string.Empty;
        }
    }
}
