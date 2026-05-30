using System;

namespace Broadcaster.Business.Models;

public class RefreshTokenObject
{
    #region Properties

    public int RefreshTokenId { get; set; }
    public string GoogleSub { get; set; } = null!;
    public string TokenHash { get; set; } = null!;
    public DateTime TokenExpires { get; set; } = DateTime.Now.AddDays(30);

    #endregion

    #region Constructors

    internal RefreshTokenObject() { }

    public RefreshTokenObject(string googleSub, string tokenHash)
    {
        GoogleSub = googleSub;
        TokenHash = tokenHash;
    }

    #endregion

    #region Sql

    public static string InitSql => """
        CREATE TABLE dat_RefreshToken (
            RefreshTokenId  INTEGER     PRIMARY KEY AUTOINCREMENT,
            GoogleSub       TEXT        NOT NULL,
            TokenHash       TEXT        NOT NULL,
            TokenExpires    INTEGER     NOT NULL
        );
        """;

    public static string InsertSql => """
        INSERT INTO dat_RefreshToken (GoogleSub, TokenHash, TokenExpires)
        VALUES (@GoogleSub, @TokenHash, @TokenExpires);
        """;

    public static string GetSql => """
        SELECT  RefreshTokenId, GoogleSub, TokenHash, TokenExpires
        FROM    dat_RefreshToken
        WHERE   TokenHash = @tokenHash
        """;

    public static string UpdateSql => """
        UPDATE  dat_RefreshToken
        SET     TokenExpires = @TokenExpires
        WHERE   RefreshTokenId = @RefreshTokenId
        """;

    public static string DeleteSql => """
        DELETE FROM dat_RefreshToken
        WHERE   RefreshTokenId = @RefreshTokenId
        """;

    public static string PruneSql => """
        DELETE FROM dat_RefreshToken
        WHERE   TokenExpires < @tokenExpires
        """;

    #endregion
}
