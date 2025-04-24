CREATE TABLE [dbo].[BlockedDomains]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[ApplicationKey] INT NOT NULL,
	[Domain] VARCHAR(500),
    [InsertedBy]		VARCHAR(100),
	[InsertedDate]		DATETIME NOT NULL DEFAULT GETDATE(),
	[UpdatedBy]			VARCHAR(100),
	[UpdatedDate]		DATETIME NOT NULL DEFAULT GETDATE(),
	[IsActive]			BIT NOT NULL DEFAULT 1
)
