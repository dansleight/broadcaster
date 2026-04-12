using System;
using Broadcaster.Business.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Broadcaster.Business;

public class UserService
{
    private readonly ILogger<UserService> _logger;
    private readonly SqliteConnection _conn;

    public UserService(ILogger<UserService> logger, DbService dbService)
    {
        _logger = logger;
        _conn = dbService.Conn;
    }

    public async Task<List<UserObject>> ListAsync()
    {
        return (await _conn.QueryAsync<UserObject>(UserObject.ListSql)).ToList();
    }

    public async Task<UserObject?> GetAsync(string email)
    {
        return (await _conn.QueryAsync<UserObject>(UserObject.GetSql, new { email })).FirstOrDefault();
    }

    public async Task InsertAsync(UserObject user, byte[] fileContent)
    {
        await _conn.ExecuteAsync(UserObject.InsertSql, user);
    }

    public Task DeleteAsync(UserObject user) => DeleteAsync(user.Email);

    public async Task DeleteAsync(string email)
    {
        await _conn.ExecuteAsync(UserObject.DeleteSql, new { email });
    }

}

public static class UserStaticRepo
{
    private static Dictionary<string, UserObject>? _users;
    private static DateTime _lastCheck = DateTime.MinValue;

    public static UserObject GetUserForAuth(IConfiguration config, string email, string name = "unknown")
    {
        DbService? dbService = null;

        if (_users == null || _lastCheck.AddHours(1) > DateTime.Now)
        {
            dbService = new DbService(config);

            List<UserObject> users = dbService.Conn.Query<UserObject>(UserObject.ListSql).ToList();
            _users = users.ToDictionary(u => u.Email);
            _lastCheck = DateTime.Now;
        }

        if (_users.TryGetValue(email, out UserObject? user))
            return user!;

        // the user isn't in the cache
        if (dbService == null)
        {
            dbService = new DbService(config);
        }

        user = dbService.Conn.Query<UserObject>(UserObject.GetSql, new { email }).SingleOrDefault();
        if (user != null)
        {
            _users.Add(user.Email, user);
            return user;
        }
        // add the user with no entitlements
        UserObject newUser = new UserObject(email, name);
        dbService.Conn.Execute(UserObject.InsertSql, newUser);
        _users.Add(newUser.Email, newUser);
        return newUser;
    }
}
