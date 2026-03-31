using System;
using System.Dynamic;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Broadcaster.Business.Models.Config;

namespace Broadcaster.Business.Services.RepoBases;

public abstract class BoundTableRepositoryBase<T> : DapperRepositoryBase, IBoundTableRepositoryBase<T> where T : class
{
    private readonly SqlGenerator<T> _sqlGenerator;

    public BoundTableRepositoryBase(
        ILogger<DapperRepositoryBase> logger,
        IOptions<DataAccessSettings> config,
        BoundTableBinder tableBinder
    ) : base(logger, config)
    {
        TableBinding = tableBinder.GetTableBinding<T>();
        _sqlGenerator = new SqlGenerator<T>(TableBinding);
    }

    protected TableBinding<T> TableBinding { get; }

    public Task<IEnumerable<T>> CachedGetAsync()
    {
        throw new NotImplementedException();
    }

    #region Get by Key

    public virtual T? GetById(int id)
    {
        Column keyColumn = GetSingleKeyColumn();

        if (keyColumn.UnderlyingType != typeof(int))
            throw new InvalidOperationException("This table's primary key column is not an integer. Please use the appropriate method.");

        string sql = _sqlGenerator.GetSelectByKeys();
        return QuerySingleOrDefault<T>(sql, new Dictionary<string, object> { { keyColumn.PropertyName, id } });
    }

    public virtual Task<T?> GetByIdAsync(int id)
    {
        Column keyColumn = GetSingleKeyColumn();

        if (keyColumn.UnderlyingType != typeof(int))
            throw new InvalidOperationException("This table's primary key column is not an integer. Please use the appropriate method.");

        string sql = _sqlGenerator.GetSelectByKeys();
        return QuerySingleOrDefaultAsync<T>(sql, new Dictionary<string, object> { { keyColumn.PropertyName, id } });
    }

    public virtual T? GetByKey(string key)
    {
        Column keyColumn = GetSingleKeyColumn();

        if (keyColumn.UnderlyingType != typeof(string))
            throw new InvalidOperationException("This table's primary key column is not a string. Please use the appropriate method.");

        string sql = _sqlGenerator.GetSelectByKeys();
        return QuerySingleOrDefault<T>(sql, new Dictionary<string, object> { { keyColumn.PropertyName, key } });
    }

    public virtual Task<T?> GetByKeyAsync(string key)
    {
        Column keyColumn = GetSingleKeyColumn();

        if (keyColumn.UnderlyingType != typeof(string))
            throw new InvalidOperationException("This table's primary key column is not a string. Please use the appropriate method.");

        string sql = _sqlGenerator.GetSelectByKeys();
        return QuerySingleOrDefaultAsync<T>(sql, new Dictionary<string, object> { { keyColumn.PropertyName, key } });
    }

    #endregion

    #region Get

    public virtual IEnumerable<T> Get()
    {
        return Get(null, null);
    }

    public virtual IEnumerable<T> Get(object? whereConditions, object? whereNotConditions = null)
    {
        string sql = _sqlGenerator.GetSelect(whereConditions, whereNotConditions);

        DynamicParameters? dynamicParameters = TableBinding.GetDynamicParameters(whereConditions, whereNotConditions);
        return Query<T>(sql, dynamicParameters);
    }

    public virtual Task<IEnumerable<T>> GetAsync()
    {
        return GetAsync(null, null);
    }

    public virtual Task<IEnumerable<T>> GetAsync(object? whereConditions, object? whereNotConditions = null)
    {
        string sql = _sqlGenerator.GetSelect(whereConditions, whereNotConditions);

        DynamicParameters? dynamicParameters = TableBinding.GetDynamicParameters(whereConditions, whereNotConditions);
        return QueryAsync<T>(sql, dynamicParameters);
    }

    #endregion

    #region Record count

    public virtual int RecordCount(object? whereConditions = null, object? whereNotConditions = null)
    {
        string sql = _sqlGenerator.GetSelectRecordCount(whereConditions, whereNotConditions);

        DynamicParameters? dynamicParameters = TableBinding.GetDynamicParameters(whereConditions, whereNotConditions);
        return ExecuteScalar<int>(sql, dynamicParameters);
    }

    public virtual Task<int> RecordCountAsync(object? whereConditions = null, object? whereNotConditions = null)
    {
        string sql = _sqlGenerator.GetSelectRecordCount(whereConditions, whereNotConditions);

        DynamicParameters? dynamicParameters = TableBinding.GetDynamicParameters(whereConditions, whereNotConditions);
        return ExecuteScalarAsync<int>(sql, dynamicParameters);
    }

    #endregion

    #region Get paged

    public virtual IEnumerable<T> GetListPaged(int pageNumber, int rowsPerPage, string orderBy, object? whereConditions = null, object? whereNotConditions = null)
    {
        string sql = _sqlGenerator.GetListPagedSql(pageNumber, rowsPerPage, orderBy, whereConditions, whereNotConditions);

        DynamicParameters? dynamicParameters = TableBinding.GetDynamicParameters(whereConditions, whereNotConditions);
        return Query<T>(sql, dynamicParameters);
    }

    public virtual Task<IEnumerable<T>> GetListPagedAsync(int pageNumber, int rowsPerPage, string orderBy, object? whereConditions = null, object? whereNotConditions = null)
    {
        string sql = _sqlGenerator.GetListPagedSql(pageNumber, rowsPerPage, orderBy, whereConditions, whereNotConditions);

        DynamicParameters? dynamicParameters = TableBinding.GetDynamicParameters(whereConditions, whereNotConditions);
        return QueryAsync<T>(sql, dynamicParameters);
    }

    #endregion

    #region Save

    public virtual async Task<T> SaveAsync(T entity)
    {
        // If single key column, we insert or update
        if (TableBinding.KeyColumns.Length == 1)
        {
            if (IsIdentityValueSet(entity, TableBinding.KeyColumns[0]))
            {
                await UpdateAsync(entity);
                return entity;
            }

            return await InsertAsync(entity);
        }

        // Composite keys
        string sql = _sqlGenerator.GetSaveStatement();
        entity = await QuerySingleAsync<T>(sql, entity);

        return entity;
    }

    #endregion

    #region Insert

    public virtual T Insert(T entity)
    {
        EnsureEntityIsValid(entity, true);
        string sql = _sqlGenerator.GetInsertStatement();
        return QuerySingle<T>(sql, entity);
    }

    public virtual Task<T> InsertAsync(T entity)
    {
        EnsureEntityIsValid(entity, true);
        string sql = _sqlGenerator.GetInsertStatement();
        return QuerySingleAsync<T>(sql, entity);
    }

    #endregion

    #region Update

    public virtual int Update(T entity)
    {
        EnsureEntityIsValid(entity);
        string sql = _sqlGenerator.GetUpdateStatement();
        int returnValue = Execute(sql, entity);
        // clear cache

        return returnValue;
    }

    public virtual async Task<int> UpdateAsync(T entity)
    {
        EnsureEntityIsValid(entity);
        string sql = _sqlGenerator.GetUpdateStatement();
        int returnValue = await ExecuteAsync(sql, entity);
        // clear cache

        return returnValue;
    }

    #endregion

    #region Delete

    public virtual async Task DeleteAsync(T entity)
    {
        string sql = _sqlGenerator.GetDeleteByKeyStatement();
        await ExecuteAsync(sql, entity).ConfigureAwait(false);
        // clear cache
    }

    public virtual async Task DeleteByIdAsync(int id)
    {
        Column keyColumn = GetSingleKeyColumn();
        string sql = _sqlGenerator.GetDeleteByKeyStatement();
        await ExecuteAsync(sql, new Dictionary<string, object> { { keyColumn.PropertyName, id } }).ConfigureAwait(false);
        // clear cache
    }

    public virtual async Task DeleteByKeyAsync(string key)
    {
        Column keyColumn = GetSingleKeyColumn();
        string sql = _sqlGenerator.GetDeleteByKeyStatement();
        await ExecuteAsync(sql, new Dictionary<string, object> { { keyColumn.PropertyName, key } }).ConfigureAwait(false);
        // clear cache
    }

    #endregion

    #region Helper Methods

    protected Column GetSingleKeyColumn()
    {
        if (TableBinding.KeyColumns.Length != 1)
            throw new InvalidOperationException($"This method is only available for tables with 1 primary key. Type {TableBinding.GetType().Name} for table {TableBinding.TableName} has {TableBinding.KeyColumns.Length} keys");

        return TableBinding.KeyColumns[0];
    }

    private static bool IsIdentityValueSet(T entity, Column keyColumn)
    {
        object? key = keyColumn.PropertyInfo.GetValue(entity);

        if (keyColumn.IsNullable) return key != null;

        switch (key)
        {
            case int x1 when x1 != 0:
            case long x2 when x2 != 0:
            case short x3 when x3 != 0:
            case string x4 when x4 != string.Empty:
                return true;
            case int:
            case long:
            case short:
            case string:
                return false;
            default:
                throw new NotImplementedException($"This type of property is not supported for a key column. You must add the appropriate case. {keyColumn.PropertyName} {keyColumn.PropertyInfo.PropertyType}");
        }
    }

    protected virtual void EnsureEntityIsValid(T entity, bool isInsert = false, List<string>? errors = null)
    {
        bool throwIfInvalid = false;

        if (errors == null)
        {
            throwIfInvalid = true;
            errors = new List<string>();
        }

        foreach (Column column in TableBinding.Columns)
        {
            if (column.ColumnAttributes.StringLengthAttribute != null && !column.ColumnAttributes.StringLengthAttribute.IsValid(StaticHelpers.GetValueDapperDbCompatible(column.PropertyInfo, entity)))
            {
                errors.Add(column.ColumnAttributes.StringLengthAttribute.FormatErrorMessage(column.PropertyName));
            }
        }

        if (isInsert)
        {
            if (TableBinding.KeyColumns.Length == 0)
                ValidateGeneratedIdentityForInsert(entity, errors, TableBinding.KeyColumns[0]);
            else
            {
                foreach (Column keyColumn in TableBinding.KeyColumns.Where(c => c.ColumnAttributes.DatabaseGeneratedAttribute is { DatabaseGeneratedOption: System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity }))
                {
                    ValidateGeneratedIdentityForInsert(entity, errors, keyColumn);
                }
            }
        }

        if (throwIfInvalid && errors.Any())
        {
            throw new ArgumentException(string.Join("\n", errors), nameof(entity));
        }
    }

    private static void ValidateGeneratedIdentityForInsert(T entity, ICollection<string> errors, Column keyColumn)
    {
        object? key = keyColumn.PropertyInfo.GetValue(entity);

        if (keyColumn.IsNullable)
        {
            if (key != null)
                errors.Add($"The key property ({keyColumn.PropertyName}) must be null");
            return;
        }

        switch (key)
        {
            case int x1 when x1 != 0:
            case long x2 when x2 != 0:
            case short x3 when x3 != 0:
                errors.Add($"The key property ({keyColumn.PropertyName}) must be 0 or be set as a nullable int with a value of null");
                break;
            case string x when x != string.Empty:
                errors.Add($"The key property ({keyColumn.PropertyName}) must be an empty string or be set as a nullable string with a value of null");
                break;
            case int:
            case long:
            case short:
            case string:
                break;
            default:
                throw new NotImplementedException($"This type of property is not supported for a key column. You must add the appropriate case. {keyColumn.PropertyName} {keyColumn.PropertyInfo.PropertyType}");
        }
    }

    #endregion
}
