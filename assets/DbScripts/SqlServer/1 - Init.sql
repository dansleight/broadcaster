

IF NOT EXISTS (
	SELECT	1
	FROM	sys.databases
	WHERE	name = 'Broadcaster'
)
BEGIN
	CREATE DATABASE Broadcaster
END
GO

USE Broadcaster
GO

IF OBJECT_ID('dat_User') IS NOT NULL
	DROP TABLE dat_User
GO

IF OBJECT_ID('enum_UserRole') IS NOT NULL
	DROP TABLE enum_UserRole
GO

IF OBJECT_ID('dat_Config') IS NOT NULL
	DROP TABLE dat_Config
GO

IF OBJECT_ID('dat_Widget') IS NOT NULL
	DROP TABLE dat_Widget
GO

-- ******************** dat_Config ********************
CREATE TABLE dat_Config (
	ConfigKey			nvarchar(100)	NOT NULL,
	ConfigValue			nvarchar(max)	NOT NULL,
	CONSTRAINT pk_dat_Config PRIMARY KEY (ConfigKey)
)
GO

-- ******************** enum_UserRole ********************
CREATE TABLE enum_UserRole (
	Role				nvarchar(50)	NOT NULL,
	CONSTRAINT pk_enum_Role PRIMARY KEY (Role)
)
GO

INSERT enum_UserRole (Role) 
VALUES 
	(N'Admin'),
	(N'User')
GO

-- ******************** dat_User ********************
CREATE TABLE dat_User (
	Email				nvarchar(250)	NOT NULL,
	Role				nvarchar(50)	NOT NULL,
	CONSTRAINT pk_dat_User PRIMARY KEY (Email, Role),
	CONSTRAINT fk_dat_User_enum_UserRole FOREIGN KEY(Role) REFERENCES enum_UserRole (Role)
)
GO

INSERT dat_User (Email, Role) 
VALUES 
	(N'test@unknown.com', N'Admin')
GO

-- ******************** dat_Widget ********************
CREATE TABLE dat_Widget (
	WidgetId			INT				NOT NULL IDENTITY(1001, 1),
	[Name]				NVARCHAR(100)	NOT NULL,
	[Description]		NVARCHAR(MAX)	NOT NULL,
	CONSTRAINT pk_dat_Widget PRIMARY KEY (WidgetId)
)
GO

SET IDENTITY_INSERT dat_Widget ON;
INSERT INTO dat_Widget (WidgetId, [Name], [Description])
VALUES
	(1, 'Gadget', 'This is a gadget, a kind of Widget'),
	(2, 'Thingy', 'This is a thingy that I can''t really describe')
GO
