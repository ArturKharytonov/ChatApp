CREATE TABLE [dbo].[Users] (
    [Id]       INT          IDENTITY (1, 1) NOT NULL,
    [Email]    VARCHAR (40) NOT NULL,
    [Username] VARCHAR (40) NOT NULL,
    [Password] VARCHAR (30) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);