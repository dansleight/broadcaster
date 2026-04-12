using System;
using Broadcaster.Business.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Broadcaster.Business;

public class UnitService
{
    private readonly ILogger<UnitService> _logger;
    private readonly SqliteConnection _conn;

    public UnitService(ILogger<UnitService> logger, DbService dbService)
    {
        _logger = logger;
        _conn = dbService.Conn;
    }

    public async Task<List<string>> ListAsync()
    {
        return (await _conn.QueryAsync<string>(UnitObject.ListSql)).ToList();
    }

    public Task InsertAsync(string unit) => _conn.ExecuteAsync(UnitObject.InsertSql, new { unit });

    public Task DeleteAsync(string unit) => _conn.ExecuteAsync(UnitObject.DeleteSql, new { unit });


}
