CREATE TABLE [dbo].[Notification](
    [Id] [bigint] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED,
    [CorrelationId] [uniqueidentifier] Not Null,
    [LearnerAccountId] [uniqueidentifier] Not Null,
    [Category] [nvarchar](255) NULL,
    [Heading] [nvarchar](255) NOT NULL,
    [Body] [nvarchar](max) NOT NULL,
    LinkUrl [nvarchar](max) NULL,
    [StatusId] [tinyint] NOT NULL Constraint [FK_Notification_NotificationStatus] Foreign Key References [dbo].[NotificationStatusType]([Id]),
    [NotificationTime] [datetime2] NOT NULL,
    [TimeToExpire] [datetime2] NOT NULL CONSTRAINT [DF_Notification_TimeToExpire] DEFAULT (dateadd(month, 3, getdate())),
    [TimeReceived] [datetime2] NOT NULL,
    [UrgencyId] [tinyint] NULL CONSTRAINT [FK_Notification_Urgency] FOREIGN KEY REFERENCES [dbo].[Urgency]([Id])
) ON [PRIMARY]
GO

CREATE INDEX [IX_Notification_CorrelationId] ON [dbo].[Notification] (
    CorrelationId
)
GO
