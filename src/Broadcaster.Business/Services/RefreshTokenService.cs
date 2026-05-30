using System;
using System.Runtime.CompilerServices;
using Broadcaster.Business.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Broadcaster.Business.Services;

public class RefreshTokenService
{
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly SqliteConnection _conn;

    public RefreshTokenService(ILogger<RefreshTokenService> logger, DbService dbService)
    {
        _logger = logger;
        _conn = dbService.Conn;

        _conn.Execute(RefreshTokenObject.PruneSql, new { tokenExpires = DateTime.Now });
    }

    public async Task<RefreshTokenObject?> GetAsync(string tokenHash)
    {
        return (await _conn.QueryAsync<RefreshTokenObject>(RefreshTokenObject.GetSql, new { tokenHash })).SingleOrDefault();
    }

    public async Task InsertAsync(RefreshTokenObject refreshToken)
    {
        await _conn.ExecuteAsync(RefreshTokenObject.InsertSql, refreshToken);
    }

    public async Task UpdateAsync(RefreshTokenObject refreshToken)
    {
        await _conn.ExecuteAsync(RefreshTokenObject.UpdateSql, refreshToken);
    }

    public async Task DeleteAsync(RefreshTokenObject refreshToken)
    {
        await _conn.ExecuteAsync(RefreshTokenObject.DeleteSql, refreshToken);
    }

    public async Task PruneAsync()
    {
        await _conn.ExecuteAsync(RefreshTokenObject.PruneSql, new { tokenExpires = DateTime.Now });
    }
}
