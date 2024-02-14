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
/****** Object:  Table [dbo].[Document]    Script Date: 10/11/2023 2:34:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Document](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[CreatedByID] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[HaveArchivedDocVersion] [bit] NOT NULL,
	[IsAllVersionsArchived] [bit] NOT NULL,
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentCategory]    Script Date: 10/11/2023 2:46:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentCategory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](max) NOT NULL,
 CONSTRAINT [PK_DocumentCategory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentVersionHistory]    Script Date: 10/11/2023 2:48:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentVersionHistory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentID] [int] NOT NULL,
	[Version] [varchar](50) NOT NULL,
	[UpdatedByID] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[ArchivedDate] [datetime] NULL,
	[Attachment] [varbinary](max) NOT NULL,
	[Type] [varchar](10) NOT NULL,
	[IsArchived] [bit] NOT NULL,
 CONSTRAINT [PK_DocumentVersionHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[User] ON
INSERT [dbo].[User] ([ID], [UserRoleID], [FullName], [Username], [Email], [Password]) VALUES (1, 1, N'Admin', N'admin', N'admin@gmail.com', N'33354741122871651676713774147412831195')
INSERT [dbo].[User] ([ID], [UserRoleID], [FullName], [Username], [Email], [Password]) VALUES (2, 2, N'Staff', N'staff', N'staff@gmail.com', N'18833213210117723916811824913021616923162239')
SET IDENTITY_INSERT [dbo].[User] OFF
GO

SET IDENTITY_INSERT [dbo].[UserRole] ON
INSERT [dbo].[UserRole] ([ID], [Name]) VALUES (1, N'Admin')
INSERT [dbo].[UserRole] ([ID], [Name]) VALUES (2, N'Staff')
SET IDENTITY_INSERT [dbo].[UserRole] OFF
GO

SET IDENTITY_INSERT [dbo].[Page] ON
INSERT [dbo].[Page] ([ID], [Name], [AccessName]) VALUES (1, N'Dashboard', N'Dashboard')
INSERT [dbo].[Page] ([ID], [Name], [AccessName]) VALUES (2, N'Repository', N'Repository')
INSERT [dbo].[Page] ([ID], [Name], [AccessName]) VALUES (3, N'Archive', N'Archive')
INSERT [dbo].[Page] ([ID], [Name], [AccessName]) VALUES (4, N'User Settings', N'UserSettings')
SET IDENTITY_INSERT [dbo].[Page] OFF
GO

SET IDENTITY_INSERT [dbo].[RoleAccessPage] ON
/** Admin **/
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (1, 1, 1)
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (2, 1, 2)
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (3, 1, 3)
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (4, 1, 4)

/** Staff **/
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (5, 2, 1)
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (6, 2, 2)
INSERT [dbo].[RoleAccessPage] ([ID], [UserRoleID], [PageID]) VALUES (7, 2, 3)
SET IDENTITY_INSERT [dbo].[RoleAccessPage] OFF
GO

SET IDENTITY_INSERT [dbo].[DocumentCategory] ON
INSERT [dbo].[DocumentCategory] ([ID], [Name]) VALUES (1, N'Other Documents')
INSERT [dbo].[DocumentCategory] ([ID], [Name]) VALUES (2, N'Case Documents')
INSERT [dbo].[DocumentCategory] ([ID], [Name]) VALUES (3, N'Client Documents')
INSERT [dbo].[DocumentCategory] ([ID], [Name]) VALUES (4, N'Legal Research')
INSERT [dbo].[DocumentCategory] ([ID], [Name]) VALUES (5, N'Contracts and Agreements')
INSERT [dbo].[DocumentCategory] ([ID], [Name]) VALUES (6, N'Court Orders and Judgments')
INSERT [dbo].[DocumentCategory] ([ID], [Name]) VALUES (7, N'Financial Documents')
INSERT [dbo].[DocumentCategory] ([ID], [Name]) VALUES (8, N'Legal Forms')
INSERT [dbo].[DocumentCategory] ([ID], [Name]) VALUES (9, N'Regulatory Compliance Documents')
INSERT [dbo].[DocumentCategory] ([ID], [Name]) VALUES (10, N'Intellectual Property Documents')
SET IDENTITY_INSERT [dbo].[DocumentCategory] OFF
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

ALTER TABLE [dbo].[Document] WITH CHECK ADD CONSTRAINT [FK_Document_DocumentCategory] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[DocumentCategory] ([ID])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_DocumentCategory]
GO

ALTER TABLE [dbo].[Document] WITH CHECK ADD CONSTRAINT [FK_Document_User] FOREIGN KEY([CreatedByID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_User]
GO

ALTER TABLE [dbo].[DocumentVersionHistory] WITH CHECK ADD CONSTRAINT [FK_DocumentVersionHistory_User] FOREIGN KEY([UpdatedByID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[DocumentVersionHistory] CHECK CONSTRAINT [FK_DocumentVersionHistory_User]
GO