using System;
using System.Globalization;
using System.Reflection;

namespace Broadcaster.Business.Services.RepoBases;

public interface ISqlGenerator
{
    string GetSelectByKeys();
    string GetSelect(object? whereConditions, object? whereNotConditions);
    string GetListPagedSql(int pageNumber, int rowsPerPage, string orderby, object? whereConditions, object? whereNotConditions);
    string GetSelectRecordCount(object? whereConditions, object? whereNotConditions);
    string GetInsertStatement();
    string GetUpdateStatement(params PropertyInfo[] ignoredProperties);
    string GetDeleteByKeyStatement();
    string? WhereClause(object? whereConditions, object? whereNotConditions);
    string GetSaveStatement();
}

public class SqlGenerator<T> : ISqlGenerator where T : class
{
    private readonly TableBinding<T> _tableBinding;
    private string TableName => $"[{_tableBinding.TableName}]";
    private string Schema => $"[{_tableBinding.Schema ?? "dbo"}]";

    public SqlGenerator(TableBinding<T> tableBinding)
    {
        _tableBinding = tableBinding;
    }

    public string GetSelect(object? whereConditions, object? whereNotConditions)
    {
        string? whereClause = WhereClause(whereConditions, whereNotConditions);

        if (!string.IsNullOrEmpty(whereClause))
        {
            return $"""
                SELECT
                {_tableBinding.SelectColumns}
                FROM {Schema}.{TableName}
                WHERE {whereClause}
                """;
        }
        return $"""
            SELECT
            {_tableBinding.SelectColumns}
            FROM {Schema}.{TableName}
            """;
    }

    public string GetSelectByKeys()
    {
        return $"""
            SELECT
            {_tableBinding.SelectColumns}
            FROM {Schema}.{TableName}
            WHERE {_tableBinding.KeyColumnsWhereClause}
            """;
    }

    public string GetInsertStatement()
    {
        string selectByKeys;
        if (_tableBinding.KeyColumns.Length == 1)
        {
            string keyColumnName = _tableBinding.KeyColumns[0].Name;

            selectByKeys = $"""
                    DECLARE @NewID decimal(38, 0) = SCOPE_IDENTITY();

                    SELECT
                    {_tableBinding.SelectColumns}
                    FROM {Schema}.{TableName}
                    WHERE {keyColumnName} = @NewID;
                """;
        }
        else
        {
            selectByKeys = GetSelectByKeys();
        }

        return $"""
                INSERT INTO {Schema}.{TableName} (
                {_tableBinding.InsertColumns}
                )
                VALUES (
                {_tableBinding.InsertColumnParameters}
                );

                {selectByKeys}
            """;
    }

    public string GetSaveStatement()
    {
        if (_tableBinding.KeyColumns.Length <= 1) throw new InvalidOperationException($"This method is only for tables with a composite key. {_tableBinding.TableName} has {_tableBinding.KeyColumns.Length} key");

        IEnumerable<Column> columns = _tableBinding.Columns.ToArray();
        string propertyVariables = "\t" + string.Join(",\n\t\t", columns.Select(c => $"@{c.PropertyName}"));
        string columnNames = "\t" + string.Join("\n\t", columns.Select(c => $"{c.Name}"));
        string targetMatch = "\t" + string.Join("\n\tAND ", _tableBinding.KeyColumns.Select(c => $"target.{c.Name} = source.{c.Name}"));
        IEnumerable<Column> columnsForUpdate = _tableBinding.GetColumnsForUpdate(null);
        string matched = "";
        if (columnsForUpdate.Any())
        {
            matched = $"""

                        WHEN MATCHED THEN
                        UPDATE SET
                            {string.Join(",\n\t", columnsForUpdate.Select(c => $"target.{c.Name} = source.{c.Name}"))}
                      """;
        }
        string insertValues = "\t" + string.Join("\n , ", columns.Select(c => $"source.{c.Name}"));

        string selectByKeys = GetSelectByKeys();

        return $"""
                MERGE INTO {Schema}.{TableName} AD target
                USING (
                    VALUES (
                    {propertyVariables}
                    )
                ) AS source (
                    {columnNames}
                )
                ON (
                {targetMatch}
                ){matched}
                WHEN NOT MATCHED THEN
                    INSERT (
                    {columnNames}
                    )
                    VALUES (
                    {insertValues}
                    );
                
                {selectByKeys}
            """;
    }

    public string GetDeleteByKeyStatement()
    {
        return $"""
            DELETE FROM {Schema}.{TableName}
            WHERE {_tableBinding.KeyColumnsWhereClause}
        """;
    }

    public string GetListPagedSql(int pageNumber, int rowsPerPage, string orderby, object? whereConditions, object? whereNotConditions)
    {
        if (pageNumber < 1) throw new InvalidOperationException("Page must be greater than 0");

        string? whereClause = WhereClause(whereConditions, whereNotConditions);

        return !string.IsNullOrWhiteSpace(whereClause) ?
            $"""
             SELECT
             {_tableBinding.SelectColumns}
             FROM {Schema}.{TableName}
             WHERE {whereClause}
             ORDER BY {orderby}
             OFFSET ({pageNumber} - 1) * {rowsPerPage} ROWS
             FETCH NEXT {rowsPerPage} ROWS ONLY;
            """
            :
            $"""
             SELECT
             {_tableBinding.SelectColumns}
             FROM {Schema}.{TableName}
             ORDER BY {orderby}
             OFFSET ({pageNumber} - 1) * {rowsPerPage} ROWS
             FETCH NEXT {rowsPerPage} ROWS ONLY;
            """;
    }

    public string GetSelectRecordCount(object? whereConditions, object? whereNotConditions)
    {
        string? whereClause = WhereClause(whereConditions, whereNotConditions);
        return !string.IsNullOrWhiteSpace(whereClause) ?
            $"""
             SELECT COUNT(1)
             FROM {Schema}.{TableName}
             WHERE {whereClause}
            """
            :
            $"""
             SELECT COUNT(1)
             FROM {Schema}.{TableName}
            """;
    }

    public string GetUpdateStatement(params PropertyInfo[] ignoredProperties)
    {
        return $"""
                UPDATE  {Schema}.{TableName}
                SET     {string.Join(",\n", _tableBinding.GetColumnsForUpdate(ignoredProperties).Select(c => $"{c.Name} = @{c.PropertyName}"))}
                WHERE   {_tableBinding.KeyColumnsWhereClause}
        """;
    }

    public string? WhereClause(object? whereConditions, object? whereNotConditions)
    {
        Dictionary<Column, object?>? whereDictionary = _tableBinding.GetColumnDictionaryFromWhereConditions(whereConditions);
        Dictionary<Column, object?>? whereNotDictionary = _tableBinding.GetColumnDictionaryFromWhereConditions(whereNotConditions);

        return WhereClause(whereDictionary, whereNotDictionary);
    }

    private string? WhereClause(Dictionary<Column, object?>? whereDictionary, Dictionary<Column, object?>? whereNotDictionary)
    {
        if (whereDictionary == null && whereNotDictionary == null) return null;

        List<string> columnstrings = [];

        if (whereDictionary != null)
        {
            columnstrings.AddRange(whereDictionary.Select(keyValuePair =>
            {
                Column column = keyValuePair.Key;
                object? value = StaticHelpers.ConvertValueDapperDbCompatibleInt(column.PropertyInfo, keyValuePair.Value);

                if (value == null || value == DBNull.Value) return $"{column.Name} IS NULL";

                if (value.GetType().IsArray) return $"{column.Name} IN @{column.PropertyName}";

                return $"{column.Name} = @{column.PropertyName}";
            }));
        }

        if (whereNotDictionary != null)
        {
            columnstrings.AddRange(whereNotDictionary.Select(keyValuePair =>
            {
                Column column = keyValuePair.Key;
                object? value = StaticHelpers.ConvertValueDapperDbCompatibleInt(column.PropertyInfo, keyValuePair.Value);

                if (value == null || value == DBNull.Value) return $"{column.Name} IS NOT NULL";

                if (value.GetType().IsArray) return $"{column.Name} NOT IN @{column.PropertyName}";

                return $"{column.Name} = @{column.PropertyName}";
            }));
        }

        return string.Join("\nAND ", columnstrings);
    }


}
