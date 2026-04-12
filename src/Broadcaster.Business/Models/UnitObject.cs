using System;
using System.Net.NetworkInformation;

namespace Broadcaster.Business;

public class UnitObject
{
    #region Properties

    public string Unit { get; set; } = null!;

    #endregion

    #region Sql

    public static string InitSql => """
        CREATE TABLE lu_Unit (
            Unit        TEXT            PRIMARY KEY
        );
        INSERT INTO lu_Unit (Unit)
        VALUES ('Stake');
        """;

    public static string InsertSql => """
        INSERT INTO lu_Unit (Unit)
        VALUES (@unit)
        """;

    public static string ListSql => """
        SELECT  Unit
        FROM    lu_Unit
        ORDER BY Unit
        """;

    public static string DeleteSql => """
        DELETE FROM lu_Unit
        WHERE   Unit = @unit
        """;

    #endregion
}
