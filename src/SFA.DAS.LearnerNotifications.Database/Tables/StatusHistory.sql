CREATE TABLE [dbo].[StatusHistory](
    [StatusHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
    [NotificationId] [bigint] NULL,
    [Status] [tinyint] NULL,
    [ChangeDate] [datetime] NULL
) ON [PRIMARY];
GO

ALTER TABLE [dbo].[StatusHistory] ADD PRIMARY KEY CLUSTERED 
(
    [StatusHistoryId] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
GO