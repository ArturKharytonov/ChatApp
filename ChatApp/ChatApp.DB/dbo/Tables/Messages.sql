CREATE TABLE [dbo].[Messages] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Content]  NVARCHAR (MAX) NOT NULL,
    [SentAt]   DATETIME2 (7)  NOT NULL,
    [RoomId]   INT            NOT NULL,
    [SenderId] INT            NOT NULL,
    CONSTRAINT [PK__Messages__3214EC0707BA7D0D] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RoomId_Messages] FOREIGN KEY ([RoomId]) REFERENCES [dbo].[Rooms]([Id]),
    CONSTRAINT [FK_SenderId_Messages] FOREIGN KEY ([SenderId]) REFERENCES [dbo].[AspNetUsers]([Id])
);



GO
CREATE NONCLUSTERED INDEX [IX_Messages_SenderId]
    ON [dbo].[Messages]([SenderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_RoomId]
    ON [dbo].[Messages]([RoomId] ASC);

