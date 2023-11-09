CREATE TABLE [dbo].[Friends] (
    [Id]           INT IDENTITY (1, 1) NOT NULL,
    [FirstUserId]  INT NOT NULL,
    [SecondUserId] INT NOT NULL,
    CONSTRAINT [PK__Friends__3214EC076A668A68] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FirstFriend] FOREIGN KEY ([FirstUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_SecondFriend] FOREIGN KEY ([SecondUserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_Friends_SecondUserId]
    ON [dbo].[Friends]([SecondUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Friends_FirstUserId]
    ON [dbo].[Friends]([FirstUserId] ASC);

