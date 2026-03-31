using System;
using System.Reflection;

namespace Broadcaster.Business.Services.RepoBases;

public static class StaticHelpers
{

    public static Type GetUnderlyingType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;

    public static object? GetValueDapperDbCompatible(PropertyInfo property, object? obj)
    {
        object? value = property.GetValue(obj);
        return ConvertValueDapperDbCompatibleInt(property, value);
    }

    internal static object? ConvertValueDapperDbCompatible(PropertyInfo property, object? value) => ConvertValueDapperDbCompatibleInt(property, value);

    internal static object? ConvertValueDapperDbCompatible(object? value)
    {
        if (value == null) return null;

        if (value.GetType().GetUnderlyingType() == typeof(DateTime))
        {
            if ((DateTime)value == DateTime.MinValue)
                return System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            return (DateTime)value;
        }

        return value;
    }

    internal static object? ConvertValueDapperDbCompatibleInt(PropertyInfo property, object? value)
    {
        if (value == null) return null;

        if (property.PropertyType.GetUnderlyingType().IsEnum)
        {
            if (value.GetType().IsArray && value is System.Collections.IEnumerable enumerable)
            {
                return (
                    from object enumValue in enumerable select enumValue.ToString()!
                ).ToArray();
            }
            return value.ToString();
        }

        if (property.PropertyType.GetUnderlyingType() == typeof(DateTime))
        {
            if ((DateTime)value == DateTime.MinValue) return System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            return (DateTime)value;
        }

        return value;
    }
}
