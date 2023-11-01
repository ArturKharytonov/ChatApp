CREATE TABLE [dbo].[AspNetUsers](
  [Id] INT IDENTITY(1, 1) NOT NULL,
  [UserName] [nvarchar](256) NOT NULL,
  [NormalizedUserName] [nvarchar](256) NOT NULL,
  [Email] [nvarchar](256) NOT NULL,
  [NormalizedEmail] [nvarchar](256) NOT NULL,
  [EmailConfirmed] [bit] NULL,
  [PasswordHash] [nvarchar](max) NULL,
  [SecurityStamp] [nvarchar](max) NULL,
  [ConcurrencyStamp] [nvarchar](max) NULL,
  [PhoneNumber] [nvarchar](max) NULL,
  [PhoneNumberConfirmed] [bit] NULL,
  [TwoFactorEnabled] [bit] NULL,
  [LockoutEnd] [datetimeoffset](7) NULL,
  [LockoutEnabled] [bit] NULL,
  [AccessFailedCount] [int] NULL,
 CONSTRAINT [PK_AspNetUsers1] PRIMARY KEY CLUSTERED 
(
  [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]