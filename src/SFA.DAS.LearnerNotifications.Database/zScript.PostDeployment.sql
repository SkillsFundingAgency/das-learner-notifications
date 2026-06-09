/*
--------------------------------------------------------------------------------------
Post-Deployment Script 
--------------------------------------------------------------------------------------
*/

SET NOCOUNT ON;
GO


MERGE INTO [dbo].[StatusType] AS Target
USING (VALUES
(1	, N'Unread'),
(2	, N'Acknowledged'),
(3	, N'Hidden'),
(4	, N'Expired')
) AS Source ([Id],[Description])
ON (Target.[Id] = Source.[Id])
WHEN MATCHED AND
  ( NULLIF(Source.[Description], Target.[Description]) IS NOT NULL) THEN
 UPDATE SET [Description] = Source.[Description]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([Id],[Description]) VALUES(Source.[Id],Source.[Description])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;
 GO

 MERGE INTO [dbo].[UrgencyType] AS Target
USING (VALUES
(1	, N'Low'),
(2	, N'Medium'),
(3	, N'High')
) AS Source ([Id],[Description])
ON (Target.[Id] = Source.[Id])
WHEN MATCHED AND
  ( NULLIF(Source.[Description], Target.[Description]) IS NOT NULL) THEN
 UPDATE SET [Description] = Source.[Description]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([Id],[Description]) VALUES(Source.[Id],Source.[Description])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;
 GO