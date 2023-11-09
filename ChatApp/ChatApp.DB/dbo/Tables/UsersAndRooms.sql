CREATE TABLE [dbo].[RoomUser] (
    [RoomsId] INT NOT NULL,
    [UsersId] INT NOT NULL,
    CONSTRAINT [PK_RoomUser] PRIMARY KEY CLUSTERED ([RoomsId] ASC, [UsersId] ASC),
    CONSTRAINT [FK_RoomUser_AspNetUsers_UsersId] FOREIGN KEY ([UsersId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoomUser_Rooms_RoomsId] FOREIGN KEY ([RoomsId]) REFERENCES [dbo].[Rooms] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_RoomUser_UsersId]
    ON [dbo].[RoomUser]([UsersId] ASC);

