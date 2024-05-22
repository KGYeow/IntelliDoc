USE [IntelliDocDB]
GO
/****** Object:  Table [dbo].[User]    Script Date: 7/11/2023 1:29:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserRoleID] [int] NOT NULL,
	[FullName] [nvarchar](max) NOT NULL,
	[Username] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[ProfilePhoto] [varbinary](max) NULL,
	[Password] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 7/11/2023 1:29:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Page]    Script Date: 7/11/2023 8:17:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Page](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[AccessName] [varchar](MAX) NULL,
 CONSTRAINT [PK_Page] PRIMARY KEY CLUSTERED
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleAccessPage]    Script Date: 7/11/2023 8:17:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleAccessPage](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserRoleID] [int] NOT NULL,
	[PageID] [int] NOT NULL,
 CONSTRAINT [PK_RoleAccessPage] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 20/11/2023 9:25:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[Title] [varchar](MAX) NOT NULL,
	[Description] [varchar](MAX) NOT NULL,
	[IsRead] [bit] NOT NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Document]    Script Date: 10/11/2023 2:34:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Document](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[Description] [varchar](max) NULL,
	[Category] [varchar](MAX) NOT NULL,
	[CurrentVersion] [int] NOT NULL,
	[CreatedByID] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedByID] [int] NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[Type] [varchar](10) NOT NULL,
	[HaveArchivedDocVersion] [bit] NOT NULL,
	[IsAllVersionsArchived] [bit] NOT NULL,
	[IsRelatedDoc] [bit] NOT NULL,
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentUserAction]    Script Date: 7/4/2023 2:52:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentUserAction](
	[UserID] [int] NOT NULL,
	[DocumentID] [int] NOT NULL,
	[IsFlagged] [bit] NOT NULL,
 CONSTRAINT [PK_DocumentUserAction] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[DocumentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentVersionHistory]    Script Date: 10/11/2023 2:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentVersionHistory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentID] [int] NOT NULL,
	[Version] [int] NOT NULL,
	[ModifiedByID] [int] NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[ArchivedDate] [datetime] NULL,
	[Attachment] [varbinary](max) NOT NULL,
	[IsArchived] [bit] NOT NULL,
 CONSTRAINT [PK_DocumentVersionHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentRelationship]    Script Date: 19/5/2024 2:48:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentRelationship](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentMainID] [int] NOT NULL,
	[DocumentRelatedID] [int] NOT NULL,
 CONSTRAINT [PK_DocumentRelationship] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[User] ON
INSERT [dbo].[User] ([ID], [UserRoleID], [FullName], [Username], [Email], [IsActive], [Password]) VALUES (1, 1, N'Admin', N'admin', N'admin@gmail.com', 1, N'33354741122871651676713774147412831195')
INSERT [dbo].[User] ([ID], [UserRoleID], [FullName], [Username], [Email], [IsActive], [Password]) VALUES (2, 2, N'Staff', N'staff', N'staff@gmail.com', 1, N'18833213210117723916811824913021616923162239')
INSERT [dbo].[User] ([ID], [UserRoleID], [FullName], [Username], [Email], [IsActive], [Password]) VALUES (3, 1, N'Yeow Kok Guan', N'yeowkokguan', N'yeowkokguan@gmail.com', 1, N'160571149248862170126514117612720113242')
INSERT [dbo].[User] ([ID], [UserRoleID], [FullName], [Username], [Email], [IsActive], [Password]) VALUES (4, 2, N'Ling Shao Doo', N'lingshaodoo', N'lingshaodoo@gmail.com', 1, N'2142253019722020443249135998418116895189247')
INSERT [dbo].[User] ([ID], [UserRoleID], [FullName], [Username], [Email], [IsActive], [Password]) VALUES (5, 2, N'Liong Man Chun', N'liongmanchun', N'liongmanchun@gmail.com', 1, N'156226206159207254641657402445991252174221')
INSERT [dbo].[User] ([ID], [UserRoleID], [FullName], [Username], [Email], [IsActive], [Password]) VALUES (6, 2, N'Lim Jie Yi', N'limjieyi', N'limjieyi@gmail.com', 1, N'10678240146222119291141534611411212712925241')
INSERT [dbo].[User] ([ID], [UserRoleID], [FullName], [Username], [Email], [IsActive], [Password]) VALUES (7, 2, N'Li Jun Tong', N'lijuntong', N'lijuntong@gmail.com', 1, N'35249758822711016422141151208145121181205122')
SET IDENTITY_INSERT [dbo].[User] OFF
GO

SET IDENTITY_INSERT [dbo].[UserRole] ON
INSERT [dbo].[UserRole] ([ID], [Name]) VALUES (1, N'Admin')
INSERT [dbo].[UserRole] ([ID], [Name]) VALUES (2, N'Staff')
SET IDENTITY_INSERT [dbo].[UserRole] OFF
GO

SET IDENTITY_INSERT [dbo].[Page] ON
INSERT [dbo].[Page] ([ID], [Name], [AccessName]) VALUES (1, N'Dashboard', N'Dashboard')
INSERT [dbo].[Page] ([ID], [Name], [AccessName]) VALUES (2, N'Repository', N'DocumentRepository')
INSERT [dbo].[Page] ([ID], [Name], [AccessName]) VALUES (3, N'Flag', N'DocumentFlag')
INSERT [dbo].[Page] ([ID], [Name], [AccessName]) VALUES (4, N'Archive', N'DocumentArchive')
INSERT [dbo].[Page] ([ID], [Name], [AccessName]) VALUES (5, N'User Setting', N'ConfigurationUserSetting')
SET IDENTITY_INSERT [dbo].[Page] OFF
GO

SET IDENTITY_INSERT [dbo].[RoleAccessPage] ON
/** Admin **/
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (1, 1, 1)
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (2, 1, 2)
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (3, 1, 3)
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (4, 1, 4)
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (5, 1, 5)
/** Staff **/
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (6, 2, 1)
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (7, 2, 2)
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (8, 2, 3)
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (9, 2, 4)
SET IDENTITY_INSERT [dbo].[RoleAccessPage] OFF
GO

ALTER TABLE [dbo].[User] WITH CHECK ADD CONSTRAINT [FK_User_UserRole] FOREIGN KEY([UserRoleID])
REFERENCES [dbo].[UserRole] ([ID])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_UserRole]
GO

ALTER TABLE [dbo].[RoleAccessPage] WITH CHECK ADD CONSTRAINT [FK_RoleAccessPage_Page] FOREIGN KEY([PageID])
REFERENCES [dbo].[Page] ([ID])
GO
ALTER TABLE [dbo].[RoleAccessPage] CHECK CONSTRAINT [FK_RoleAccessPage_Page]
GO
ALTER TABLE [dbo].[RoleAccessPage] WITH CHECK ADD CONSTRAINT [FK_RoleAccessPage_UserRole] FOREIGN KEY([UserRoleID])
REFERENCES [dbo].[UserRole] ([ID])
GO
ALTER TABLE [dbo].[RoleAccessPage] CHECK CONSTRAINT [FK_RoleAccessPage_UserRole]
GO

ALTER TABLE [dbo].[Notification] WITH CHECK ADD CONSTRAINT [FK_Notification_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_User]
GO

ALTER TABLE [dbo].[Document] WITH CHECK ADD CONSTRAINT [FK_Document_CreatedByUser] FOREIGN KEY([CreatedByID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_CreatedByUser]
GO
ALTER TABLE [dbo].[Document] WITH CHECK ADD CONSTRAINT [FK_Document_ModifiedByUser] FOREIGN KEY([ModifiedByID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_ModifiedByUser]
GO

ALTER TABLE [dbo].[DocumentUserAction] WITH CHECK ADD CONSTRAINT [FK_DocumentUserAction_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[DocumentUserAction] CHECK CONSTRAINT [FK_DocumentUserAction_User]
GO
ALTER TABLE [dbo].[DocumentUserAction] WITH CHECK ADD CONSTRAINT [FK_DocumentUserAction_Document] FOREIGN KEY([DocumentID])
REFERENCES [dbo].[Document] ([ID])
GO
ALTER TABLE [dbo].[DocumentUserAction] CHECK CONSTRAINT [FK_DocumentUserAction_Document]
GO

ALTER TABLE [dbo].[DocumentVersionHistory] WITH CHECK ADD CONSTRAINT [FK_DocumentVersionHistory_Document] FOREIGN KEY([DocumentID])
REFERENCES [dbo].[Document] ([ID])
GO
ALTER TABLE [dbo].[DocumentVersionHistory] CHECK CONSTRAINT [FK_DocumentVersionHistory_Document]
GO
ALTER TABLE [dbo].[DocumentVersionHistory] WITH CHECK ADD CONSTRAINT [FK_DocumentVersionHistory_User] FOREIGN KEY([ModifiedByID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[DocumentVersionHistory] CHECK CONSTRAINT [FK_DocumentVersionHistory_User]
GO

ALTER TABLE [dbo].[DocumentRelationship] WITH CHECK ADD CONSTRAINT [FK_DocumentRelationship_DocumentMain] FOREIGN KEY([DocumentMainID])
REFERENCES [dbo].[Document] ([ID])
GO
ALTER TABLE [dbo].[DocumentRelationship] CHECK CONSTRAINT [FK_DocumentRelationship_DocumentMain]
GO
ALTER TABLE [dbo].[DocumentRelationship] WITH CHECK ADD CONSTRAINT [FK_DocumentRelationship_DocumentRelated] FOREIGN KEY([DocumentRelatedID])
REFERENCES [dbo].[Document] ([ID])
GO
ALTER TABLE [dbo].[DocumentRelationship] CHECK CONSTRAINT [FK_DocumentRelationship_DocumentRelated]
GO