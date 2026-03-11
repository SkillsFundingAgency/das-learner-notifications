CREATE TABLE [dbo].[Notification](
    [NotificationId] [bigint] IDENTITY(1,1) NOT NULL,
    [CorrelationId] [uniqueidentifier] NULL,
    [LearnerAccountId] [uniqueidentifier] NULL,
    [Category] [nvarchar](255) NULL,
    [Heading] [nvarchar](255) NULL,
    [Body] [nvarchar](max) NULL,
    [StatusId] [tinyint] NULL,
    [NotificationTime] [datetime] NULL,
    [TimeToExpire] [datetime] NULL,
    [TimeReceived] [datetime] NULL,
    [Link] [nvarchar](500) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Notification] ADD PRIMARY KEY CLUSTERED 
(
    [NotificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO