CREATE TABLE [dbo].[Friends]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1), 
    [FirstUserId] INT NOT NULL, 
    [SecondUserId] INT NOT NULL,
    CONSTRAINT [FK_FirstFriend] FOREIGN KEY ([FirstUserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_SecondFriend] FOREIGN KEY ([SecondUserId]) REFERENCES [Users]([Id])
);
