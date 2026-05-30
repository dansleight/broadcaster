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

    public Task<IEnumerable<UserObject>> ListAsync()
        => _conn.QueryAsync<UserObject>(UserObject.ListSql);


    public Task<UserObject?> GetAsync(string googleSub)
        => _conn.QuerySingleOrDefaultAsync<UserObject>(UserObject.GetSql, new { googleSub });


    public Task<UserObject?> GetByEmailAsync(string email)
        => _conn.QuerySingleOrDefaultAsync<UserObject>(UserObject.GetByEmailSql, new { email });


    public Task InsertAsync(UserObject user, byte[] fileContent)
        => _conn.ExecuteAsync(UserObject.InsertSql, user);

    public Task DeleteAsync(UserObject user) => DeleteAsync(user.Email);

    public Task DeleteAsync(string email)
        => _conn.ExecuteAsync(UserObject.DeleteSql, new { email });

    public async Task<UserObject> GetOrCreateUserAsync(string googleSub, string email, string name, string? givenName, string? familyName, string? picture, string refreshToken)
    {
        UserObject? user = await _conn.QuerySingleOrDefaultAsync<UserObject>(UserObject.GetSql, new { googleSub });
        if (user == null)
        {
            user = new UserObject(googleSub, email, name, givenName, familyName, picture);
            await _conn.ExecuteAsync(UserObject.InsertSql, user);
        }
        else
        {
            if (user.Email != email
                || user.Name != name
                || user.GivenName != givenName
                || user.FamilyName != familyName
                || user.Picture != picture)
            {
                user.Email = email;
                user.Name = name;
                user.GivenName = givenName ?? user.GivenName;
                user.FamilyName = familyName ?? user.FamilyName;
                user.Picture = picture ?? user.Picture;
                await _conn.ExecuteAsync(UserObject.UpdateSql, user);
            }
        }
        await _conn.ExecuteAsync(UserObject.SaveRefreshTokenSql, new { googleSub, refreshToken });
        return user;
    }

}

