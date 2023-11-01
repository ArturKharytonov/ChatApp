CREATE TABLE [dbo].[Messages]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Content] NVARCHAR(MAX) NOT NULL,
	[SentAt] TIMESTAMP NOT NULL,
	[RoomId] INT NOT NULL,
	[SenderId] INT NOT NULL,
	CONSTRAINT [FK_SenderId_Messages] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers]([Id]),
	CONSTRAINT [FK_RoomId_Messages] FOREIGN KEY ([RoomId]) REFERENCES [Rooms]([Id])
)
