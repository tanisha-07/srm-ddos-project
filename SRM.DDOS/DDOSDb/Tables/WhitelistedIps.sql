CREATE TABLE [dbo].[WhitelistedIps]
(
	Id INT NOT NULL IDENTITY(1,1),
	[ApplicationKey] INT NOT NULL,
    IPAddress VARCHAR(45) NOT NULL, -- Supports IPv4 and IPv6
    Reason NVARCHAR(255) NULL,
    [InsertedBy]		VARCHAR(100),
	[InsertedDate]		DATETIME NOT NULL DEFAULT GETDATE(),
	[UpdatedBy]			VARCHAR(100),
	[UpdatedDate]		DATETIME NOT NULL DEFAULT GETDATE(),
	[IsActive]			BIT NOT NULL DEFAULT 1
)
