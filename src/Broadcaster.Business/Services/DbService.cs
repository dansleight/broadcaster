using System;
using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.Extensions.Options;
using Broadcaster.Business.Models.Config;
using Broadcaster.Business.Models;
using Microsoft.Extensions.Configuration;

namespace Broadcaster.Business;

public class DbService
{
    private readonly string _connectionString;
    private readonly SqliteConnection _connection;
    private static bool _initChecked = false;
    private static object _lock = new object();

    public DbService(IConfiguration config)
    {
        _connectionString = $"Data Source={GetDatabasePath(config)}";
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

                List<string> tables = _connection.Query<string>(checkExistsSql).ToList();

                if (!tables.Contains("lu_Unit")) _connection.Execute(UnitObject.InitSql);
                if (!tables.Contains("dat_Placeholder")) _connection.Execute(PlaceholderObject.InitSql);
                if (!tables.Contains("dat_User")) _connection.Execute(UserObject.InitSql);
                _initChecked = true;
            }
        }
    }

    public SqliteConnection Conn => _connection;


    #region Helpers

    internal static string GetDatabasePath(IConfiguration config)
    {
        // Production / systemd path
        var path = config.GetSection("Database:Path").Value;

        // Development override (optional but nice)
        if (string.IsNullOrEmpty(path) &&
            config.GetSection("Database:UseDevelopmentPath").Value == "False")
        {
            path = Path.Combine("/var/lib/broadcaster", "broadcaster.db");
        }

        // Fallback if still empty (never fails)
        path ??= Path.Combine(AppContext.BaseDirectory, "data", "broadcaster.db");

        Directory.CreateDirectory(Path.GetDirectoryName(path)!); // safety net

        return path;
    }

    #endregion
}
