CREATE TABLE [dbo].[UsersAndRooms]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[UserId] INT NOT NULL,
	[RoomId] INT NOT NULL,
	CONSTRAINT [FK_UserId_UsersAndRooms] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
	CONSTRAINT [FK_RoomId_UsersAndRooms] FOREIGN KEY ([RoomId]) REFERENCES [Rooms]([Id])
)