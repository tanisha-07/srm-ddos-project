CREATE TABLE [dbo].[Application]
(
	[ApplicationKey] INT NOT NULL PRIMARY KEY,
	[ApplicationName]	VARCHAR(200) NOT NULL,
    [InsertedBy]		VARCHAR(100),
	[InsertedDate]		DATETIME NOT NULL DEFAULT GETDATE(),
	[UpdatedBy]			VARCHAR(100),
	[UpdatedDate]		DATETIME NOT NULL DEFAULT GETDATE(),
	[IsActive]			BIT NOT NULL DEFAULT 1
)
