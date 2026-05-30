using System;
using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.Extensions.Options;
using Broadcaster.Business.Models.Config;
using Broadcaster.Business.Models;
using Microsoft.Extensions.Configuration;
using Broadcaster.Common;

namespace Broadcaster.Business;

public class DbService
{
    private readonly string _connectionString;
    private readonly SqliteConnection _connection;
    private static bool _initChecked = false;
    private static object _lock = new object();

    public DbService(IConfiguration config, ArtifactHelper artifactHelper)
    {
        _connectionString = $"Data Source={artifactHelper.DatabasePath}";
        _connection = new SqliteConnection(_connectionString);

        lock (_lock)
        {
            if (_initChecked != true)
            {
                const string checkExistsSql = """
                SELECT name 
                FROM sqlite_master 
                WHERE type = 'table' 
                """;

                _connection.Open();

                List<string> tables = _connection.Query<string>(checkExistsSql).ToList();

                if (!tables.Contains("lu_Unit")) _connection.Execute(UnitObject.InitSql);
                if (!tables.Contains("dat_Placeholder")) _connection.Execute(PlaceholderObject.InitSql);
                if (!tables.Contains("dat_User")) _connection.Execute(UserObject.InitSql);
                if (!tables.Contains("dat_RefreshToken")) _connection.Execute(RefreshTokenObject.InitSql);
                _initChecked = true;
            }
        }
    }

    public SqliteConnection Conn => _connection;

}
