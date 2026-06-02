CREATE TABLE [dbo].[NotificationStatus](
    [StatusId] [tinyint] NOT NULL CONSTRAINT [PK_NotificationStatus] PRIMARY KEY CLUSTERED,
    [Description] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO
