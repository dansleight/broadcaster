using System;
using Newtonsoft.Json;

namespace Broadcaster.Business;

public class UserObject
{
    #region Properties

    public string GoogleSub { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? Picture { get; set; }
    public List<string> Units { get; set; } = new();
    public List<UserRole> Roles { get; set; } = new();

    #endregion

    #region Utility Properites

    [JsonIgnore]
    public string UnitsAsString
    {
        get
        {
            return string.Join(',', Units);
        }
        set
        {
            Units = value.Split(',').ToList();
        }
    }

    [JsonIgnore]
    public string RolesAsString
    {
        get => string.Join(',', Roles.Select(r => r.ToString()));
        set
        {
            Roles = value.Split(',')
                .Select(r => (UserRole?)(Enum.TryParse<UserRole>(r, out UserRole userRole) ? userRole : null))
                .Where(r => r.HasValue)
                .Select(r => (UserRole)r!)
                .ToList();
        }
    }

    #endregion

    #region Constructors

    internal UserObject() { }

    public UserObject(
        string googleSub,
        string email,
        string name,
        string? givenName = null,
        string? familyName = null,
        string? picture = null)
    {
        GoogleSub = googleSub;
        Email = email;
        Name = name;
        GivenName = givenName;
        FamilyName = familyName;
        Picture = picture;
    }

    #endregion

    #region Sql

    public static string InitSql => """
        CREATE TABLE dat_User (
            GoogleSub       TEXT        PRIMARY KEY,
            Email           TEXT        NOT NULL,
            Name            TEXT        NOT NULL,
            GivenName       TEXT        NULL,
            FamilyName      TEXT        NULL,
            Picture         TEXT        NULL,
            UnitsAsString   TEXT        NOT NULL,
            RolesAsString   TEXT        NOT NULL,
            RefreshToken    TEXT        NULL
        );
        """;

    public static string InsertSql => """
        INSERT INTO dat_User (GoogleSub, Email, Name, GivenName, FamilyName, Picture, UnitsAsString, RolesAsString)
        VALUES (@GoogleSub, @Email, @Name, @GivenName, @FamilyName, @Picture, @UnitsAsString, @RolesAsString)
        """;

    public static string UpdateSql => """
        UPDATE  dat_User
        SET     Email = @Email,
                Name = @Name,
                GivenName = @GivenName,
                FamilyName = @FamilyName,
                Picture = @Picture,
                UnitsAsString = @UnitsAsString,
                RolesAsString = @RolesAsString
        WHERE   GoogleSub = @GoogleSub
        """;

    public static string ListSql => """
        SELECT  GoogleSub, Email, Name, GivenName, FamilyName, Picture, UnitsAsString, RolesAsString
        FROM    dat_User
        ORDER BY Name
        """;

    public static string GetSql => """
        SELECT  GoogleSub, Email, Name, GivenName, FamilyName, Picture, UnitsAsString, RolesAsString
        FROM    dat_User
        WHERE   GoogleSub = @googleSub
        """;

    public static string GetByEmailSql => """
        SELECT  GoogleSub, Email, Name, GivenName, FamilyName, Picture, UnitsAsString, RolesAsString
        FROM    dat_User
        WHERE   Email = @email
        """;

    public static string DeleteSql => """
        DELETE FROM dat_User
        WHERE   GoogleSub = @googleSub
        """;

    public static string SaveRefreshTokenSql => """
        UPDATE  dat_User
        SET     RefreshToken = @refreshToken
        WHERE   GoogleSub = @googleSub
        """;

    public static string GetRefreshTokenSql => """
        SELECT  RefreshToken
        FROM    dat_User
        WHERE   GoogleSub = @googleSub
        """;

    #endregion
}
