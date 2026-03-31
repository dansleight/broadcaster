using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Broadcaster.Business.Services.RepoBases;

internal class ColumnAttributes
{
    internal ColumnAttributes(ColumnAttribute columnAttribute, PropertyInfo propertyInfo)
    {
        ColumnAttribute = columnAttribute;
        KeyAttribute = GetAttribute<KeyAttribute>(propertyInfo);
        NotMappedAttribute = GetAttribute<NotMappedAttribute>(propertyInfo);
        ReadOnlyAttribute = GetAttribute<ReadOnlyAttribute>(propertyInfo);
        DatabaseGeneratedAttribute = GetAttribute<DatabaseGeneratedAttribute>(propertyInfo);
        RequiredAttribute = GetAttribute<RequiredAttribute>(propertyInfo);
        StringLengthAttribute = GetAttribute<StringLengthAttribute>(propertyInfo);
    }

    public ColumnAttribute ColumnAttribute { get; }

    private static TAttribute? GetAttribute<TAttribute>(ICustomAttributeProvider attributeProvider)
        => attributeProvider.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>().FirstOrDefault();

    internal KeyAttribute? KeyAttribute { get; }

    internal NotMappedAttribute? NotMappedAttribute { get; }

    internal ReadOnlyAttribute? ReadOnlyAttribute { get; }

    internal DatabaseGeneratedAttribute? DatabaseGeneratedAttribute { get; }

    internal RequiredAttribute? RequiredAttribute { get; }

    internal StringLengthAttribute? StringLengthAttribute { get; }
}