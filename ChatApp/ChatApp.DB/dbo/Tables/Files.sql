CREATE TABLE [dbo].[Files]
(
	[Id] VARCHAR(100) NOT NULL PRIMARY KEY, --Open AI Generated Id
	[Name] NVARCHAR(100) NOT NULL,
	[GroupId] INT            NOT NULL,
	[UserId] INT            NOT NULL,
	CONSTRAINT [FK_Files_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Rooms]([Id]),
	CONSTRAINT [FK_Files_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id])
)
