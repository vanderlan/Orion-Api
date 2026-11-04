using System.ComponentModel;

namespace Company.Orion.Domain.Core.Extensions;

public static class EnumExtensions
{
    /// <summary>Returns the <see cref="DescriptionAttribute"/> of an enum member, or an empty string when absent.</summary>
    public static string Description<T>(this T source) where T : struct, Enum
    {
        var fieldInfo = typeof(T).GetField(source.ToString());

        if (fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] { Length: > 0 } attributes)
            return attributes[0].Description;

        return string.Empty;
    }
}
