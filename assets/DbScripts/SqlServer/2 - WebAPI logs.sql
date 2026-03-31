USE Broadcaster
GO

-- ******************** dat_SerilogLogs ********************
IF OBJECT_ID('dat_SerilogLogs') IS NOT NULL
	DROP TABLE dat_SerilogLogs
GO

CREATE TABLE dat_SerilogLogs (
	Id				INT					IDENTITY(1, 1) NOT NULL,
	Message			NVARCHAR(MAX)		NULL,
	MessageTemplate	NVARCHAR(MAX)		NULL,
	Level			NVARCHAR(MAX)		NULL,
	TimeStamp		NVARCHAR(MAX)		NULL,
	Exception		NVARCHAR(MAX)		NULL,
	LogEvent		NVARCHAR(MAX)		NULL,
	Username		NVARCHAR(50)		NULL,
	ApplicationName	NVARCHAR(50)		NULL,
	MachineName		NVARCHAR(50)		NULL,
	CONSTRAINT pk_dat_SerilogLogs PRIMARY KEY (Id)
)
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sproc_trim_dat_SerilogLogs')
BEGIN
	DROP PROCEDURE sproc_trim_dat_SerilogLogs
END
GO

CREATE PROCEDURE sproc_trim_dat_SerilogLogs
	@Test VARCHAR(25) = 'No'
AS
	DECLARE	@DefaultLogRetentionDays		INT,
			@InformationLogRetentionDays	INT,
			@ErrorLogRetentionDays			INT,
			@WarningLogRetentionDays		INT,
			@RowCount						BIGINT,
			@ErrorMessage					VARCHAR(4000);

	BEGIN TRY
		-- Insert default parameters into dat_Config if they don't exist
		IF NOT EXISTS (SELECT * FROM dat_Config WHERE ConfigKey = 'DefaultLogRetentionDays')
		BEGIN
			INSERT INTO dat_Config
			SELECT 'DefaultLogRetentionDays', '366'
		END
		
		IF NOT EXISTS (SELECT * FROM dat_Config WHERE ConfigKey = 'InformationLogRetentionDays')
		BEGIN
			INSERT INTO dat_Config
			SELECT 'InformationLogRetentionDays', '0'
		END

		IF NOT EXISTS (SELECT * FROM dat_Config WHERE ConfigKey = 'ErrorLogRetentionDays')
		BEGIN
			INSERT INTO dat_Config
			SELECT 'ErrorLogRetentionDays', '0'
		END

		IF NOT EXISTS (SELECT * FROM dat_Config WHERE ConfigKey = 'WarningLogRetentionDays')
		BEGIN
			INSERT INTO dat_Config
			SELECT 'WarningLogRetentionDays', '0'
		END

		-- Determine how long to keep data, using Default where value is 0 or empty
		SELECT	@DefaultLogRetentionDays = CAST(ConfigValue AS INT)
		FROM	dat_Config
		WHERE	ConfigKey = 'DefaultLogRetentionDays';

		SELECT	@InformationLogRetentionDays = 
			CASE WHEN ISNULL(TRIM(ConfigValue), '0') IN ('0', '') 
				THEN CAST(@DefaultLogRetentionDays AS INT)
				ELSE CAST(ConfigValue AS INT)
			END
		FROM	dat_Config
		WHERE	ConfigKey = 'InformationLogRetentionDays';

		SELECT	@ErrorLogRetentionDays = 
			CASE WHEN ISNULL(TRIM(ConfigValue), '0') IN ('0', '') 
				THEN CAST(@DefaultLogRetentionDays AS INT)
				ELSE CAST(ConfigValue AS INT)
			END
		FROM	dat_Config
		WHERE	ConfigKey = 'ErrorLogRetentionDays';

		SELECT	@WarningLogRetentionDays = 
			CASE WHEN ISNULL(TRIM(ConfigValue), '0') IN ('0', '') 
				THEN CAST(@DefaultLogRetentionDays AS INT)
				ELSE CAST(ConfigValue AS INT)
			END
		FROM	dat_Config
		WHERE	ConfigKey = 'WarningLogRetentionDays';

		IF LEFT(UPPER(@Test), 1) IN ('Y', 'T') -- should handle Y, y, Yes, YES, yes, T, t, True, TRUE, true, etc.
		BEGIN
			SELECT	'@DefaultLogRetentionDays', RetentionDays = @DefaultLogRetentionDays
			UNION
			SELECT	'@InformationLogRetentionDays', RetentionDays = @InformationLogRetentionDays
			UNION
			SELECT	'@ErrorLogRetentionDays', RetentionDays = @ErrorLogRetentionDays
			UNION
			SELECT	'@WarningLogRetentionDays', RetentionDays = @WarningLogRetentionDays
		END

		-- Remove Information Records
		DELETE 
		FROM	dat_SerilogLogs
		WHERE	[TimeStamp] < DATEADD(DAY, -@InformationLogRetentionDays, GETDATE())
		AND		[Level] = 'Information';

		SELECT	@RowCount = @@RowCount;
		INSERT INTO dat_SerilogLogs ([Message], MessageTemplate, [Level], [TimeStamp], Exception, LogEvent, Username)
		VALUES (
			'Procedure: sproc_trim_dat_SerilogLogs removed ' + CAST(@RowCount AS VARCHAR(25)) + ' Information level rows from dat_SerilogLogs',
			'Procedure: sproc_trim_dat_SerilogLogs removed {Count} {Level} level rows from dat_SerilogLogs',
			'Information',
			GETDATE(),
			NULL,
			'{"Properties":{"Count":"' + CAST(@RowCount AS VARCHAR(25)) + '","Level":"Information"}}',
			SYSTEM_USER
		);

		-- Remove Error Records
		DELETE 
		FROM	dat_SerilogLogs
		WHERE	[TimeStamp] < DATEADD(DAY, -@ErrorLogRetentionDays, GETDATE())
		AND		[Level] = 'Error';

		SELECT	@RowCount = @@RowCount;
		INSERT INTO dat_SerilogLogs ([Message], MessageTemplate, [Level], [TimeStamp], Exception, LogEvent, Username)
		VALUES (
			'Procedure: sproc_trim_dat_SerilogLogs removed ' + CAST(@RowCount AS VARCHAR(25)) + ' Error level rows from dat_SerilogLogs',
			'Procedure: sproc_trim_dat_SerilogLogs removed {Count} {Level} level rows from dat_SerilogLogs',
			'Information',
			GETDATE(),
			NULL,
			'{"Properties":{"Count":"' + CAST(@RowCount AS VARCHAR(25)) + '","Level":"Error"}}',
			SYSTEM_USER
		);

		-- Remove Warning Records
		DELETE 
		FROM	dat_SerilogLogs
		WHERE	[TimeStamp] < DATEADD(DAY, -@WarningLogRetentionDays, GETDATE())
		AND		[Level] = 'Warning';

		SELECT	@RowCount = @@RowCount;
		INSERT INTO dat_SerilogLogs ([Message], MessageTemplate, [Level], [TimeStamp], Exception, LogEvent, Username)
		VALUES (
			'Procedure: sproc_trim_dat_SerilogLogs removed ' + CAST(@RowCount AS VARCHAR(25)) + ' Warning level rows from dat_SerilogLogs',
			'Procedure: sproc_trim_dat_SerilogLogs removed {Count} {Level} level rows from dat_SerilogLogs',
			'Information',
			GETDATE(),
			NULL,
			'{"Properties":{"Count":"' + CAST(@RowCount AS VARCHAR(25)) + '","Level":"Warning"}}',
			SYSTEM_USER
		);

		IF LEFT(UPPER(@Test), 1) IN ('Y', 'T') -- should handle Y, y, Yes, YES, yes, T, t, True, TRUE, true, etc.
		BEGIN
			SELECT	TOP 3 Id, [Message], MessageTemplate, [Level], [TimeStamp], Exception, LogEvent, Username
			FROM	dat_SerilogLogs
			WHERE	LEFT(MessageTemplate, 37) = 'Procedure: sproc_trim_dat_SerilogLogs'
			ORDER BY [TimeStamp]
		END

	END TRY
	BEGIN CATCH
		SELECT	@ErrorMessage = 'Procedure: sproc_trim_dat_SerilogLogs failed at line: ' + CAST(ERROR_LINE() AS VARCHAR(25)) + ' - with error: ' + ERROR_MESSAGE();
		INSERT INTO dat_SerilogLogs ([Message], MessageTemplate, [Level], [TimeStamp], Exception, LogEvent, Username)
		VALUES (
			'Procedure: sproc_trim_dat_SerilogLogs removed ' + CAST(@RowCount AS VARCHAR(25)) + ' Warning level rows from dat_SerilogLogs',
			'Procedure: sproc_trim_dat_SerilogLogs removed {Count} {Level} level rows from dat_SerilogLogs',
			'Information',
			GETDATE(),
			NULL,
			'{"Properties":{"Count":"' + CAST(@RowCount AS VARCHAR(25)) + '","Level":"Warning"}}',
			SYSTEM_USER
		);
	END CATCH