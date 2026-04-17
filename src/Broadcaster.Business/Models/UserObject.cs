using System;
using Newtonsoft.Json;

namespace Broadcaster.Business;

public class UserObject
{
    #region Properties

    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
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

    public UserObject(string email, string name)
    {
        Email = email;
        Name = name;
    }

    #endregion

    #region Sql

    public static string InitSql => """
        CREATE TABLE dat_User (
            Email           TEXT        PRIMARY KEY,
            Name            TEXT        NOT NULL,
            UnitsAsString   TEXT        NOT NULL,
            RolesAsString   TEXT        NOT NULL
        );
        INSERT INTO dat_User (Email, Name, UnitsAsString, RolesAsString)
        VALUES
            ('dan.sleight@eaglerock.net', 'Dan Sleight', 'West Stake', 'Admin');
        """;

    public static string InsertSql => """
        INSERT INTO dat_User (Email, UnitsAsString, RolesAsString)
        VALUES (@Email, @UnitsAsString, @RolesAsString)
        """;

    public static string ListSql => """
        SELECT  *
        FROM    dat_User
        ORDER BY Name
        """;

    public static string GetSql => """
        SELECT  *
        FROM    dat_User
        WHERE   Email = @email
        """;

    public static string DeleteSql => """
        DELETE FROM dat_User
        WHERE   Email = @email
        """;

    #endregion
}
