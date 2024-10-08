CREATE TABLE [dbo].[Rooms] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (30) NOT NULL,
    [AssistantId] VARCHAR(100),
    [CreatorId] INT NOT NULL,
    CONSTRAINT [PK__Rooms__3214EC07511992BF] PRIMARY KEY CLUSTERED ([Id] ASC)
);
