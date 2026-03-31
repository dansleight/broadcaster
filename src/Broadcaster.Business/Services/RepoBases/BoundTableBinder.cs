using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Broadcaster.Business.Services.RepoBases;

public class BoundTableBinder
{
    private static ConcurrentDictionary<Type, ITableBinding>? TableBindings { get; set; }

    public BoundTableBinder(Assembly[] assemblies)
    {
        TableBindings = new ConcurrentDictionary<Type, ITableBinding>();

        PopulateTableBindings(assemblies);
    }

    private static void PopulateTableBindings(IEnumerable<Assembly> assemblies)
    {
        IEnumerable<Type> tableTypes = GetTypesWithTableAttribute(assemblies);

        foreach (Type tableType in tableTypes)
            SetTableBindingForType(tableType);
    }

    private static void SetTableBindingForType(Type tableType)
    {
        IEnumerable<Column> columns = from propertyInfo in tableType.GetProperties()
                                      let columnAttribute = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false).Cast<ColumnAttribute>().SingleOrDefault()
                                      where columnAttribute != null
                                      select new Column(propertyInfo, new ColumnAttributes(columnAttribute, propertyInfo));

        Type tableBindingType = typeof(TableBinding<>).MakeGenericType(tableType);
        var tableBinding = (ITableBinding)Activator.CreateInstance(tableBindingType, columns)!;
        TableBindings![tableType] = tableBinding;
    }

    public TableBinding<T> GetTableBinding<T>() where T : class
    {
        if (typeof(T).GetCustomAttributes(typeof(TableAttribute), false).Cast<TableAttribute>().SingleOrDefault() == null)
            throw new InvalidOperationException($"The given type {typeof(T)} does not have a {nameof(TableAttribute)} and cannot be used with this method.");

        return (TableBinding<T>)TableBindings![typeof(T)];
    }

    private static IEnumerable<Type> GetTypesWithTableAttribute(IEnumerable<Assembly> assemblies) =>
        TypeMapping.GetExportedTypes(assemblies.ToArray()).typesFound.Where(type => type.IsDefined(typeof(TableAttribute), false));
}
