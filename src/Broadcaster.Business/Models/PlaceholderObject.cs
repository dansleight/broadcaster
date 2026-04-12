using System;

namespace Broadcaster.Business;

public class PlaceholderObject
{
    #region Properties

    public int PlaceholderId { get; set; }
    public string? Unit { get; set; } = null!;
    public string Name { get; set; } = null!;

    #endregion

    #region Constructors

    internal PlaceholderObject() { }

    public PlaceholderObject(string? unit, string name)
    {
        Unit = unit;
        Name = name;
    }

    #endregion

    #region Sql

    public static string InitSql => """
        CREATE TABLE IF NOT EXISTS dat_Placeholder (
            PlaceholderId   INTEGER             PRIMARY KEY AUTOINCREMENT,
            Unit            TEXT                NULL,
            Name            TEXT                NOT NULL,
            FileContent     BLOB                NOT NULL
        );
        """;

    public static string InsertSql => """
        INSERT INTO dat_Placeholder (Unit, Name, FileContent)
        VALUES (@Unit, @Name, @FileContent);
        """;

    public static string ListSql => """
        SELECT  PlaceholderId, Unit, Name
        FROM    dat_Placeholder
        ORDER BY Name
        """;

    public static string ListForUnitSql => """
        SELECT  PlaceholderId, Unit, Name
        FROM    dat_Placeholder
        WHERE   Unit = @unit
        OR      Unit IS NULL
        ORDER BY Name
        """;

    public static string ListForNoUnitSql => """
        SELECT  PlaceholderId, Unit, Name
        FROM    dat_Placeholder
        WHERE   Unit IS NULL
        ORDER BY Name
        """;

    public static string GetSql => """
        SELECT  PlaceholderId, Unit, Name
        FROM    dat_Placeholder
        WHERE   PlaceholderId = @placeholderId
        """;

    public static string GetImageSql => """
        SELECT  FileContent
        FROM    dat_Placeholder
        WHERE   PlaceholderId = @placeholderId
        """;

    public static string DeleteSql => """
        DELETE FROM dat_Placeholder
        WHERE   PlaceholderId = @placeholderId
        """;

    #endregion
}
