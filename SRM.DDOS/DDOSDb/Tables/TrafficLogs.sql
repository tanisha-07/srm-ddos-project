CREATE TABLE [dbo].[TrafficLogs]
(
	[TrafficLogKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[ApplicationKey] INT NULL,
	[Domain] VARCHAR(500),
	[IPAddress] VARCHAR(45),
	[InsertedDate] DATETIME NOT NULL DEFAULT GETDATE()	
)
