using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Dapper;

#pragma warning disable EF1001 // Internal EF Core API usage.

namespace Broadcaster.Business.Services.RepoBases;

public interface ITableBinding { }

public class TableBinding<T> : ITableBinding where T : class
{
    private readonly Type[] _validKeyColumnTypes = [typeof(string), typeof(int), typeof(long), typeof(short)];
    private IReadOnlyList<Column>? _columnsForUpdate;
    private string? _keyColumnsWhereClause;
    private string? _selectColumns;
    private string? _insertColumns;
    private string? _insertColumnParameters;

    public TableBinding(IEnumerable<Column> columns)
    {

        Table = typeof(T).GetCustomAttributes(typeof(TableAttribute), false).Cast<TableAttribute>().Single();

        Columns = columns.ToArray();
        KeyColumns = columns.Where(c => c.IsKeyColumn).ToArray();

        if (columns.Count() == 0) return; // don't take this seriously

        if (KeyColumns.Length == 0)
            throw new ArgumentException($"Class {typeof(T).Name} with the {nameof(TableAttribute)} must have at least one property with a {nameof(ColumnAttribute)} also marked with {nameof(KeyAttribute)}");

        List<string> errors = (
            from keyColumn in KeyColumns
            where !_validKeyColumnTypes.Contains(keyColumn.UnderlyingType)
            select $"Property {keyColumn.PropertyName} has the key attribute and is of type {keyColumn.Type} which is not supported as a valid key type").ToList();

        if (errors.Count > 0)
        {
            string validColumnTypes = string.Join(", ", (IEnumerable<Type>)_validKeyColumnTypes);
            throw new InvalidOperationException($"The bound table type {nameof(T)} is invalid.\nKeyColumnsmust be of type {validColumnTypes}. May be nullable.\n{string.Join("\n", errors)}");
        }

    }

    private TableAttribute Table { get; }
    public Column[] Columns { get; }
    public Column[] KeyColumns { get; }
    public string SelectColumns => _selectColumns ??= "    " + string.Join("\n    , ", Columns.Select(c => c.Name));
    public string InsertColumns => _insertColumns ??= "    " + string.Join("\n    , ", Columns.Where(c => c.IncludeInInsertStatement).Select(c => c.Name));
    public string InsertColumnParameters => _insertColumnParameters ??= "    @" + string.Join("\n  , @", Columns.Where(c => c.IncludeInInsertStatement).Select(c => c.PropertyName));
    public string KeyColumnsWhereClause => _keyColumnsWhereClause ??= string.Join("\nAND ", KeyColumns.Select(c => $"{c.Name} = @{c.PropertyName}"));

    public string TableName => Table.Name;

    public string? Schema => Table.Schema;

    internal IEnumerable<Column> GetColumnsForUpdate(PropertyInfo[]? ignoredProperties)
    {
        IReadOnlyList<Column> columns = _columnsForUpdate ??= Columns.Where(c => c.IncludeInUpdateStatement).ToList();

        return ignoredProperties == null || ignoredProperties.Length == 0 ? columns : columns.Where(c => !ignoredProperties.Contains(c.PropertyInfo));
    }

    internal Dictionary<Column, object?>? GetColumnDictionaryFromWhereConditions(object? whereConditions)
    {
        if (whereConditions == null) return null;

        IDictionary<string, object?> whereDictionary = whereConditions as IDictionary<string, object?>
            ?? whereConditions.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(whereConditions))!;

        ICollection<string> properties = whereDictionary.Keys;

        var columns = new Dictionary<Column, object?>(properties.Count);

        var errors = new List<string>();

        foreach (string propertyName in properties)
        {
            var column = Columns.SingleOrDefault(c => c.PropertyName.Equals(propertyName));
            if (column != null)
            {
                object? value = whereDictionary[propertyName];

                if (value != null)
                {
                    Type conditionType = value.GetType();
                    if (column.PropertyInfo.PropertyType.GetUnderlyingType() != conditionType)
                    {
                        if (conditionType.IsArray) conditionType = conditionType.GetElementType()!;
                        else if (conditionType.GetInterface(nameof(System.Collections.IEnumerable)) != null && conditionType.GenericTypeArguments.Length == 1)
                            conditionType = conditionType.GenericTypeArguments[0];

                        if (column.PropertyInfo.PropertyType.GetUnderlyingType() != conditionType)
                            errors.Add($"invalid where condition type. Property {propertyName} type is {column.PropertyInfo.PropertyType.GetUnderlyingType()}. Condition type was {conditionType}");
                    }
                }

                columns.Add(column, value);
            }
            else
            {
                errors.Add($"invalid property: {propertyName}");
            }
        }

        if (errors.Any())
            throw new ArgumentException($"{nameof(whereConditions)} contains the following errors: {string.Join(", ", errors.OrderBy(s => s))}", nameof(whereConditions));

        return columns;
    }

    internal DynamicParameters? GetDynamicParameters(params object?[] parametersArray)
    {
        if (parametersArray.All(p => p == null)) return null;

        DynamicParameters dynamicParameters = new DynamicParameters();
        List<string> errors = [];

        foreach (object? parameters in parametersArray)
        {
            if (parameters == null) continue;
            switch (parameters)
            {
                case ICollection<KeyValuePair<string, object?>> keyValuePairsNullable:
                    {
                        foreach (KeyValuePair<string, object?> keyValuePair in keyValuePairsNullable)
                        {
                            object? value = keyValuePair.Value;
                            if (value != null)
                            {
                                var column = Columns.SingleOrDefault(c => c.PropertyName.Equals(keyValuePair.Key));
                                if (column != null) value = StaticHelpers.ConvertValueDapperDbCompatible(column.PropertyInfo, value);
                                else errors.Add($"invalid property: {keyValuePair.Key}");
                            }
                            dynamicParameters.Add(keyValuePair.Key, value);
                        }
                        break;
                    }
                case ICollection<KeyValuePair<string, string>> stringValuePairs:
                    {
                        foreach (KeyValuePair<string, string> keyValuePair in stringValuePairs)
                        {
                            dynamicParameters.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                        break;
                    }
                default:
                    {
                        PropertyInfo[] properties = parameters.GetType().GetProperties();

                        foreach (PropertyInfo property in properties)
                        {
                            dynamicParameters.Add(property.Name, StaticHelpers.GetValueDapperDbCompatible(property, parameters));
                        }
                        break;
                    }
            }
        }

        if (errors.Any())
            throw new ArgumentException($"{nameof(parametersArray)} contains the following errors: {string.Join(", ", errors.OrderBy(s => s))}", nameof(parametersArray));

        return dynamicParameters;
    }
}
