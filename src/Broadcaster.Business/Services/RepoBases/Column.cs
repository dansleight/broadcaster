using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Broadcaster.Business.Services.RepoBases;

public class Column
{
    internal Column(PropertyInfo propertyInfo, ColumnAttributes columnAttributes)
    {
        PropertyInfo = propertyInfo;
        ColumnAttributes = columnAttributes;
    }

    internal PropertyInfo PropertyInfo { get; }

    internal ColumnAttributes ColumnAttributes { get; }

    internal Type UnderlyingType => PropertyInfo.PropertyType.GetUnderlyingType();

    internal Type Type => PropertyInfo.PropertyType;

    internal bool IsNullable => Nullable.GetUnderlyingType(Type) != null;

    public string Name => ColumnAttributes.ColumnAttribute.Name ?? PropertyInfo.Name;

    public string PropertyName => PropertyInfo.Name;

    internal bool IsKeyColumn => ColumnAttributes.KeyAttribute != null;

    internal bool IncludeInInsertStatement => IsWritable;

    internal bool IncludeInUpdateStatement => IsWritable;

    private bool IsWritable =>
        ColumnAttributes.KeyAttribute == null
        && ColumnAttributes.NotMappedAttribute == null
        && ColumnAttributes.ReadOnlyAttribute is not { IsReadOnly: true }
        && (ColumnAttributes.DatabaseGeneratedAttribute == null
            || ColumnAttributes.DatabaseGeneratedAttribute.DatabaseGeneratedOption == DatabaseGeneratedOption.None);

}
