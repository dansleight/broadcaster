USE Broadcaster
GO

IF OBJECT_ID('user_Broadcaster') IS NOT NULL
	DROP USER user_Broadcaster
GO

IF (SELECT SUSER_ID('user_Broadcaster')) IS NULL
	CREATE LOGIN user_Broadcaster WITH PASSWORD = 'Password!here'
GO

CREATE USER user_Broadcaster FOR LOGIN user_Broadcaster
    WITH DEFAULT_SCHEMA = dbo;  
GO  
ALTER ROLE [db_datareader] ADD MEMBER user_Broadcaster
GO
ALTER ROLE [db_datawriter] ADD MEMBER user_Broadcaster
GO