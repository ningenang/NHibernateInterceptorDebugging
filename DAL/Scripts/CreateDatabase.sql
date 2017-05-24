

IF DB_ID('lipidDb') IS NULL
BEGIN
	CREATE DATABASE lipidDb
	CONTAINMENT = NONE
	ON PRIMARY

	-- Make sure the path is correct
	(NAME = 'lipidDb_Data', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\lipidDb.mdf' )
	LOG ON
	(NAME = 'lipidDb_lOG', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\lipidDb.ldf')
END	


ALTER DATABASE [lipidDb] SET COMPATIBILITY_LEVEL = 110
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [lipidDb].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO

ALTER DATABASE [lipidDb] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [lipidDb] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [lipidDb] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [lipidDb] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [lipidDb] SET ARITHABORT OFF 
GO

ALTER DATABASE [lipidDb] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [lipidDb] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [lipidDb] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [lipidDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [lipidDb] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [lipidDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [lipidDb] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [lipidDb] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [lipidDb] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [lipidDb] SET  DISABLE_BROKER 
GO

ALTER DATABASE [lipidDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [lipidDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [lipidDb] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [lipidDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [lipidDb] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [lipidDb] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [lipidDb] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [lipidDb] SET RECOVERY FULL 
GO

ALTER DATABASE [lipidDb] SET  MULTI_USER 
GO

ALTER DATABASE [lipidDb] SET PAGE_VERIFY TORN_PAGE_DETECTION  
GO

ALTER DATABASE [lipidDb] SET DB_CHAINING OFF 
GO

ALTER DATABASE [lipidDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [lipidDb] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [lipidDb] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [lipidDb] SET  READ_WRITE 
GO


USE [lipidDb];
GO

CREATE TABLE [dbo].[Person]
(
	[PersonID]				INT				IDENTITY(1,1) NOT NULL,
	[UserIdentification]	NVARCHAR(100)	NOT NULL,
	[CreatedDate]			DATETIME		NULL,
	[CreatedByPersonID]		INT				NULL,
	[ModifiedDate]			DATETIME		NULL,
	[ModifiedByPersonID]	INT				NULL,
	[LoggedInPersonID]		INT				NULL,
	CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([PersonID] ASC)
);

CREATE TABLE [dbo].[Voyage]
(
	[VoyageID]				INT				IDENTITY(1,1) NOT NULL,
	[ShipName]				NVARCHAR(100)	NOT NULL,
	[FromLocation]			NVARCHAR(100)	NOT NULL,
	[ToLocation]			NVARCHAR(100)	NOT NULL,
	[ETD]					DATETIME2(0)	NOT NULL,
	[ETA]					DATETIME2(0)	NOT NULL,
	[CreatedDate]			DATETIME		NULL,
	[CreatedByPersonID]		INT				NULL,
	[ModifiedDate]			DATETIME		NULL,
	[ModifiedByPersonID]	INT				NULL,
	[LoggedInPersonID]		INT				NULL,
	CONSTRAINT [PK_Voyage] PRIMARY KEY CLUSTERED ([VoyageID] ASC)
);


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
ALTER DATABASE "lipidDb"
   SET RECURSIVE_TRIGGERS OFF	-- Required for the AFTER UPDATE triggers (is the MSSQL default)
GO
 
-- ===================================================================
-- Table name: Person
-- ===================================================================
 
-- AFTER INSERT trigger
-- --------------------
-- Drop trigger if it already exists.
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trigger_AFTER_INSERT_Person]'))
  DROP TRIGGER [dbo].[Trigger_AFTER_INSERT_Person]
GO
 
CREATE TRIGGER [Trigger_AFTER_INSERT_Person] ON [dbo].[Person] AFTER INSERT 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
 
	DECLARE @LPID INT
	DECLARE @PKEY INT
 
	SET @LPID = (SELECT LoggedInPersonID FROM INSERTED)
	SET @PKEY = (SELECT PersonID FROM INSERTED)
 
	-- LoggedInPersonID cannot be NULL or 0. If this is true then raise an error.
	IF @LPID <> 0 
		BEGIN
			-- Note: LoggedInPersonID is already inserted, and not needed in this update statement
			UPDATE Person SET CreatedByPersonID = @LPID, CreatedDate = GETDATE() 
			WHERE PersonID = @PKEY
		END
	ELSE
		BEGIN
			-- Raise an Ad hoc message (Ad hoc messages raise an error with msg_id = 50000)
			RAISERROR ('Person.LoggedInPersonID cannot be set to NULL for an INSERT (PersonID = %d)', 16, 1, @PKEY)
			ROLLBACK TRANSACTION
		END
END
GO
 
 
-- AFTER UPDATE trigger
-- --------------------
-- Drop trigger if it already exists.
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trigger_AFTER_UPDATE_Person]'))
  DROP TRIGGER [dbo].[Trigger_AFTER_UPDATE_Person]
GO
 
CREATE TRIGGER [Trigger_AFTER_UPDATE_Person] ON [dbo].[Person] AFTER UPDATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
 
	DECLARE @LPID INT
	DECLARE @PKEY INT
	DECLARE @COUNT INT
 
	SET @PKEY = (SELECT top 1 PersonID FROM INSERTED)
 
	-- This trigger will be fired even if no rows where effected.
	-- In such a senario, the primary key is null, and we have nothing to do!
	SET @PKEY = CASE WHEN @PKEY IS NULL THEN -1 ELSE @PKEY END
	IF @PKEY > 0
	BEGIN
		-- We have work to do. Check if more than one row have been affected.
		set @COUNT = (SELECT count(*) FROM INSERTED)
		IF @COUNT > 1
			BEGIN
				-- More than one row affected. Do the UPDATE one by one using a cursor
				-- NOTE. This require the database property RECURSIVE_TRIGGERS to be OFF
				DECLARE affectedRowsCursor CURSOR for select PersonID, LoggedInPersonID from INSERTED FOR READ ONLY
				OPEN affectedRowsCursor
				FETCH NEXT FROM affectedRowsCursor into @PKEY, @LPID
				WHILE (@@FETCH_STATUS = 0)
				BEGIN
					-- LoggedInPersonID cannot be NULL or 0. If this is true then raise an error.
					IF @LPID <> 0
						BEGIN
							UPDATE Person SET ModifiedByPersonID = @LPID, LoggedInPersonID = Null, ModifiedDate = GETDATE() 
							WHERE PersonID = @PKEY
						END
					ELSE
						BEGIN
							-- Raise an Ad hoc message (Ad hoc messages raise an error with msg_id = 50000)
							RAISERROR ('Person.LoggedInPersonID cannot be set to NULL for an UPDATE (PersonID = %d)', 16, 1, @PKEY)
							ROLLBACK TRANSACTION
						END
 
					FETCH NEXT FROM affectedRowsCursor into @PKEY, @LPID
				END
 
				CLOSE affectedRowsCursor
				DEALLOCATE affectedRowsCursor
			END
		ELSE
			BEGIN
				-- Only one row affected by the update.
				-- LoggedInPersonID cannot be NULL or 0. If this is true then raise an error.
				SET @LPID = (SELECT LoggedInPersonID FROM INSERTED)
				IF @LPID <> 0
					BEGIN
						UPDATE Person SET ModifiedByPersonID = @LPID, LoggedInPersonID = Null, ModifiedDate = GETDATE() 
						WHERE PersonID = @PKEY
					END
				ELSE
					BEGIN
						-- Raise an Ad hoc message (Ad hoc messages raise an error with msg_id = 50000)
						RAISERROR ('Person.LoggedInPersonID cannot be set to NULL for an UPDATE (PersonID = %d)', 16, 1, @PKEY)
						ROLLBACK TRANSACTION
					END
			END
	END
END
GO
 
 
-- ===================================================================
-- Table name: Voyage
-- ===================================================================
 
-- AFTER INSERT trigger
-- --------------------
-- Drop trigger if it already exists.
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trigger_AFTER_INSERT_Voyage]'))
  DROP TRIGGER [dbo].[Trigger_AFTER_INSERT_Voyage]
GO
 
CREATE TRIGGER [Trigger_AFTER_INSERT_Voyage] ON [dbo].[Voyage] AFTER INSERT 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
 
	DECLARE @LPID INT
	DECLARE @PKEY INT
 
	SET @LPID = (SELECT LoggedInPersonID FROM INSERTED)
	SET @PKEY = (SELECT VoyageID FROM INSERTED)
 
	-- LoggedInPersonID cannot be NULL or 0. If this is true then raise an error.
	IF @LPID <> 0 
		BEGIN
			-- Note: LoggedInPersonID is already inserted, and not needed in this update statement
			UPDATE Voyage SET CreatedByPersonID = @LPID, CreatedDate = GETDATE() 
			WHERE VoyageID = @PKEY
		END
	ELSE
		BEGIN
			-- Raise an Ad hoc message (Ad hoc messages raise an error with msg_id = 50000)
			RAISERROR ('Voyage.LoggedInPersonID cannot be set to NULL for an INSERT (VoyageID = %d)', 16, 1, @PKEY)
			ROLLBACK TRANSACTION
		END
END
GO
 
 
-- AFTER UPDATE trigger
-- --------------------
-- Drop trigger if it already exists.
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trigger_AFTER_UPDATE_Voyage]'))
  DROP TRIGGER [dbo].[Trigger_AFTER_UPDATE_Voyage]
GO
 
CREATE TRIGGER [Trigger_AFTER_UPDATE_Voyage] ON [dbo].[Voyage] AFTER UPDATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
 
	DECLARE @LPID INT
	DECLARE @PKEY INT
	DECLARE @COUNT INT
 
	SET @PKEY = (SELECT top 1 VoyageID FROM INSERTED)
 
	-- This trigger will be fired even if no rows where effected.
	-- In such a senario, the primary key is null, and we have nothing to do!
	SET @PKEY = CASE WHEN @PKEY IS NULL THEN -1 ELSE @PKEY END
	IF @PKEY > 0
	BEGIN
		-- We have work to do. Check if more than one row have been affected.
		set @COUNT = (SELECT count(*) FROM INSERTED)
		IF @COUNT > 1
			BEGIN
				-- More than one row affected. Do the UPDATE one by one using a cursor
				-- NOTE. This require the database property RECURSIVE_TRIGGERS to be OFF
				DECLARE affectedRowsCursor CURSOR for select VoyageID, LoggedInPersonID from INSERTED FOR READ ONLY
				OPEN affectedRowsCursor
				FETCH NEXT FROM affectedRowsCursor into @PKEY, @LPID
				WHILE (@@FETCH_STATUS = 0)
				BEGIN
					-- LoggedInPersonID cannot be NULL or 0. If this is true then raise an error.
					IF @LPID <> 0
						BEGIN
							UPDATE Voyage SET ModifiedByPersonID = @LPID, LoggedInPersonID = Null, ModifiedDate = GETDATE() 
							WHERE VoyageID = @PKEY
						END
					ELSE
						BEGIN
							-- Raise an Ad hoc message (Ad hoc messages raise an error with msg_id = 50000)
							RAISERROR ('Voyage.LoggedInPersonID cannot be set to NULL for an UPDATE (VoyageID = %d)', 16, 1, @PKEY)
							ROLLBACK TRANSACTION
						END
 
					FETCH NEXT FROM affectedRowsCursor into @PKEY, @LPID
				END
 
				CLOSE affectedRowsCursor
				DEALLOCATE affectedRowsCursor
			END
		ELSE
			BEGIN
				-- Only one row affected by the update.
				-- LoggedInPersonID cannot be NULL or 0. If this is true then raise an error.
				SET @LPID = (SELECT LoggedInPersonID FROM INSERTED)
				IF @LPID <> 0
					BEGIN
						UPDATE Voyage SET ModifiedByPersonID = @LPID, LoggedInPersonID = Null, ModifiedDate = GETDATE() 
						WHERE VoyageID = @PKEY
					END
				ELSE
					BEGIN
						-- Raise an Ad hoc message (Ad hoc messages raise an error with msg_id = 50000)
						RAISERROR ('Voyage.LoggedInPersonID cannot be set to NULL for an UPDATE (VoyageID = %d)', 16, 1, @PKEY)
						ROLLBACK TRANSACTION
					END
			END
	END
END
GO
 

 SET IDENTITY_INSERT Person ON;
 INSERT INTO Person (PersonID, UserIdentification, CreatedDate, CreatedByPersonID, ModifiedDate, ModifiedByPersonID, LoggedInPersonID)
	VALUES (1000, N'System', GETDATE(), 1000, GETDATE(), 1000, 1000);

SET IDENTITY_INSERT Person OFF;

INSERT INTO Person (UserIdentification, CreatedDate, CreatedByPersonID, ModifiedDate, ModifiedByPersonID, LoggedInPersonID)
	VALUES (N'foo@client.com', GETDATE(), 1000, GETDATE(), 1000, 1000);	

INSERT INTO	Person (UserIdentification, CreatedDate, CreatedByPersonID, ModifiedDate, ModifiedByPersonID, LoggedInPersonID)
	VALUES (N'bar@client.com', GETDATE(), 1000, GETDATE(), 1000, 1000);