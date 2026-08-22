using System;
using System.Reflection;
using System.ComponentModel;

namespace Curators.Domain.Enums;

public static class EnumExtension
{
    /// <summary>
    /// This is an extension method for Enums. It yields the "Description" attribute's value
    /// </summary>
    /// <param name="enumValue">The enum value to convert</param>
    /// <returns></returns>
    public static string ObtainDescription(this Enum enumValue)
    {
        Type type = enumValue.GetType();
        MemberInfo[] memberInfo = type.GetMember(enumValue.ToString());

        if (memberInfo.Length == 0)
            return enumValue.ToString();

        var descriptionAttribute = memberInfo[0]
            .GetCustomAttribute<DescriptionAttribute>();

        return descriptionAttribute?.Description ?? enumValue.ToString();
    }
}
