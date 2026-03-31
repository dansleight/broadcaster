using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Broadcaster.Business.Models.Config;

namespace Broadcaster.Business.Services.RepoBases;

public abstract class DapperRepositoryBase
{
    private readonly ILogger<DapperRepositoryBase> _logger;
    private readonly string _connectionString;

    protected DapperRepositoryBase(
        ILogger<DapperRepositoryBase> logger,
        IOptions<DataAccessSettings> config)
    {
        _logger = logger;
        _connectionString = config.Value.ConnectionStrings!.Single(x => x.Key == "DefaultConnection").Value;
    }

    #region Map Dapper Methods

    protected int Execute(string query, object? parameters = null) => GetConnection().Execute(query, AsDapperParams(parameters));

    protected T? ExecuteScalar<T>(string query, object? parameters = null) => GetConnection().ExecuteScalar<T>(query, AsDapperParams(parameters));

    protected IDataReader ExecuteReader(string query, object? parameters = null) => GetConnection().ExecuteReader(query, AsDapperParams(parameters));

    protected IEnumerable<dynamic> Query(string query, object? parameters = null) => GetConnection().Query(query, AsDapperParams(parameters));

    protected IEnumerable<T> Query<T>(string query, object? parameters = null) => GetConnection().Query<T>(query, AsDapperParams(parameters));

    protected IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string query, Func<TFirst, TSecond, TReturn> map, object? parameters = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => GetConnection().Query(query, map, AsDapperParams(parameters), transaction, buffered, splitOn, commandTimeout, commandType);

    protected IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string query, Func<TFirst, TSecond, TThird, TReturn> map, object? parameters = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => GetConnection().Query(query, map, AsDapperParams(parameters), transaction, buffered, splitOn, commandTimeout, commandType);

    protected IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string query, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? parameters = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => GetConnection().Query(query, map, AsDapperParams(parameters), transaction, buffered, splitOn, commandTimeout, commandType);

    protected T QueryFirst<T>(string query, object? parameters = null) => GetConnection().QueryFirst<T>(query, AsDapperParams(parameters));

    protected T? QueryFirstOrDefault<T>(string query, object? parameters = null) => GetConnection().QueryFirstOrDefault<T>(query, AsDapperParams(parameters));

    protected T QuerySingle<T>(string query, object? parameters = null) => GetConnection().QuerySingle<T>(query, AsDapperParams(parameters));

    protected T? QuerySingleOrDefault<T>(string query, object? parameters = null) => GetConnection().QuerySingleOrDefault<T>(query, AsDapperParams(parameters));

    protected Task<int> ExecuteAsync(string query, object? parameters = null) => GetConnection().ExecuteAsync(query, AsDapperParams(parameters));

    protected Task<T?> ExecuteScalarAsync<T>(string query, object? parameters = null) => GetConnection().ExecuteScalarAsync<T>(query, AsDapperParams(parameters));

    protected Task<DbDataReader> ExecuteReaderAsync(string query, object? parameters = null) => GetConnection().ExecuteReaderAsync(query, AsDapperParams(parameters));

    protected Task<IEnumerable<dynamic>> QueryAsync(string query, object? parameters = null) => GetConnection().QueryAsync(query, AsDapperParams(parameters));

    protected Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null) => GetConnection().QueryAsync<T>(query, AsDapperParams(parameters));

    protected Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string query, Func<TFirst, TSecond, TReturn> map, object? parameters = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => GetConnection().QueryAsync(query, map, AsDapperParams(parameters), transaction, buffered, splitOn, commandTimeout, commandType);

    protected Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string query, Func<TFirst, TSecond, TThird, TReturn> map, object? parameters = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => GetConnection().QueryAsync(query, map, AsDapperParams(parameters), transaction, buffered, splitOn, commandTimeout, commandType);

    protected Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string query, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? parameters = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => GetConnection().QueryAsync(query, map, AsDapperParams(parameters), transaction, buffered, splitOn, commandTimeout, commandType);

    protected Task<T> QueryFirstAsync<T>(string query, object? parameters = null) => GetConnection().QueryFirstAsync<T>(query, AsDapperParams(parameters));

    protected Task<T?> QueryFirstOrDefaultAsync<T>(string query, object? parameters = null) => GetConnection().QueryFirstOrDefaultAsync<T>(query, AsDapperParams(parameters));

    protected Task<T> QuerySingleAsync<T>(string query, object? parameters = null) => GetConnection().QuerySingleAsync<T>(query, AsDapperParams(parameters));

    protected Task<T?> QuerySingleOrDefaultAsync<T>(string query, object? parameters = null) => GetConnection().QuerySingleOrDefaultAsync<T>(query, AsDapperParams(parameters));

    #endregion

    #region Helpers

    protected DbConnection GetConnection()
    {
        DbConnection sqlConnection = new SqlConnection(_connectionString);
        return sqlConnection;
    }

    public object? AsDapperParams(object? o)
    {
        switch (o)
        {
            case null:
                return null;
            case DynamicParameters:
                return o;
            case IDictionary<string, object> or IDictionary<string, object?>:
                return o;
        }

        // https://stackoverflow.com/questions/37264655/dapper-and-enums-as-strings
        var properties = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(c => c.CanRead).ToArray();

        return properties
            .Select(c => new { Key = c.Name, Value = c.GetValue(o), Type = c.PropertyType })
            .ToDictionary(
                c => c.Key,
                c => (c.Type.IsEnum || Nullable.GetUnderlyingType(c.Type)
                    ?.IsEnum == true) ? c.Value?.ToString() : c.Value);
    }

    #endregion
}
