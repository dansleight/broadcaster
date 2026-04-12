using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;

namespace Broadcaster.Business;

public class PlaceholderService
{

    private readonly ILogger<PlaceholderService> _logger;
    private readonly SqliteConnection _conn;

    public PlaceholderService(ILogger<PlaceholderService> logger, DbService dbService)
    {
        _logger = logger;
        _conn = dbService.Conn;
    }

    public async Task<List<PlaceholderObject>> ListAsync()
    {
        return (await _conn.QueryAsync<PlaceholderObject>(PlaceholderObject.ListSql)).ToList();
    }

    public async Task<List<PlaceholderObject>> ListForUnitAsync(string unit)
        => (await _conn.QueryAsync<PlaceholderObject>(PlaceholderObject.ListForUnitSql, new { unit })).ToList();

    public async Task<List<PlaceholderObject>> ListForNoUnitAsync()
        => (await _conn.QueryAsync<PlaceholderObject>(PlaceholderObject.ListForNoUnitSql)).ToList();

    public async Task<PlaceholderObject?> GetAsync(int placeholderId)
    {
        return (await _conn.QueryAsync<PlaceholderObject>(PlaceholderObject.GetSql, new { placeholderId })).FirstOrDefault();
    }

    public async Task<byte[]?> GetImageAsync(int placeholderId)
    {
        return (await _conn.QueryAsync<byte[]>(PlaceholderObject.GetImageSql, new { placeholderId })).FirstOrDefault();
    }

    public async Task InsertAsync(PlaceholderObject placeholder, byte[] fileContent)
    {
        await _conn.ExecuteAsync(PlaceholderObject.InsertSql, new
        {
            placeholder.Unit,
            placeholder.Name,
            FileContent = fileContent
        });
    }

    public Task DeleteAsync(PlaceholderObject placeholder) => DeleteAsync(placeholder.PlaceholderId);

    public async Task DeleteAsync(int placeholderId)
    {
        await _conn.ExecuteAsync(PlaceholderObject.DeleteSql, new { placeholderId });
    }
}
