INSERT INTO [dbo].[Status] ([Id], [Description])
VALUES 
    (1, 'Unread'),
    (2, 'Acknowledged'),
    (3, 'Hidden'),
    (4, 'Expired');
GO

SET IDENTITY_INSERT [dbo].[Notification] ON;
GO

-- Insert sample notifications
INSERT INTO [dbo].[Notification] (
    [NotificationId],
    [CorrelationId],
    [LearnerAccountId],
    [Category],
    [Heading],
    [Body],
    [StatusId],
    [NotificationTime],
    [TimeToExpire],
    [TimeReceived],
    [Link],
    [UrgencyId]
)
VALUES
-- Notification 1: System Alert
(1001, NEWID(), 'A3B2C1D4-E5F6-7890-ABCD-EF1234567890', 'SystemAlert', 
 'Important System Update', 
 'We will be performing maintenance on Saturday at 2 AM. The system will be unavailable for approximately 2 hours.',
  1, -- Sent
  DATEADD(day, -1, GETUTCDATE()), -- Yesterday
  DATEADD(day, 30, GETUTCDATE()), -- Expires in 30 days
  DATEADD(day, -2, GETUTCDATE()),
  '/system/maintenance',
  1), -- Medium urgency

-- Notification 2: Training Reminder
(1002, NEWID(), 'A3B2C1D4-E5F6-7890-ABCD-EF1234567890', 'TrainingReminder', 
 'Mandatory Training Due', 
 'Your mandatory health and safety training is due in 7 days. Please complete it before the deadline.',
  2, -- Read
  DATEADD(day, -3, GETUTCDATE()), -- 3 days ago
  DATEADD(day, 7, GETUTCDATE()), -- Expires in 7 days
  DATEADD(day, -5, GETUTCDATE()),
  '/training/mandatory',
  2), -- High urgency

-- Notification 3: Apprenticeship Update
(1003, NEWID(), 'B4C3D2E1-F6E5-8901-BCDE-FA2345678901', 'ApprenticeshipUpdate', 
 'New Learning Materials Available', 
 'New learning materials for Module 3 have been uploaded to your portal. Please review them before your next session.',
  0, -- Pending
  GETUTCDATE(), -- Now
  DATEADD(day, 14, GETUTCDATE()), -- Expires in 14 days
  DATEADD(hour, -1, GETUTCDATE()),
  '/learning/module-3',
  0), -- Low urgency

-- Notification 4: Assessment Notification
(1004, NEWID(), 'B4C3D2E1-F6E5-8901-BCDE-FA2345678901', 'Assessment', 
 'Assessment Scheduled', 
 'Your end-of-module assessment has been scheduled for next Friday at 10 AM. Please ensure you are prepared.',
  3, -- Expired
  DATEADD(day, -10, GETUTCDATE()), -- 10 days ago
  DATEADD(day, -1, GETUTCDATE()), -- Expired yesterday
  DATEADD(day, -12, GETUTCDATE()),
  '/assessments/upcoming',
  1), -- Medium urgency

-- Notification 5: Feedback Request
(1005, NEWID(), 'C5D4E3F2-1728-9012-CDEF-AB3456789012', 'Feedback', 
 'Please Provide Feedback', 
 'We value your feedback on the recent training session. Please take 5 minutes to complete our survey.',
  4, -- Failed
  DATEADD(day, -2, GETUTCDATE()), -- 2 days ago
  DATEADD(day, 5, GETUTCDATE()), -- Expires in 5 days
  DATEADD(day, -2, GETUTCDATE()),
  '/feedback/session-123',
  0), -- Low urgency

-- Notification 6: Account Verification
(1006, NEWID(), 'C5D4E3F2-1728-9012-CDEF-AB3456789012', 'Account', 
 'Verify Your Account', 
 'Please verify your email address to complete your account setup and access all features.',
  1, -- Sent
  GETUTCDATE(), -- Now
  DATEADD(day, 3, GETUTCDATE()), -- Expires in 3 days
  DATEADD(minute, -30, GETUTCDATE()),
  '/account/verify',
  2), -- High urgency

-- Notification 7: Notification with NULL Link
(1007, NEWID(), 'D6E5F4A3-2839-0123-DE4A-BC4567890123', 'SystemAlert', 
 'Welcome to the Platform', 
 'Welcome to our apprenticeship platform! We are excited to have you on board.',
  2, -- Read
  DATEADD(day, -30, GETUTCDATE()), -- 30 days ago
  DATEADD(day, 365, GETUTCDATE()), -- Expires in 1 year
  DATEADD(day, -30, GETUTCDATE()),
  NULL,
  NULL); -- NULL urgency
GO

-- Turn off identity insert
SET IDENTITY_INSERT [dbo].[Notification] OFF;
GO

-- Insert status history for the notifications
INSERT INTO [dbo].[StatusHistory] (
    [StatusHistoryId],
    [NotificationId],
    [Status],
    [ChangeDate]
)
VALUES
-- Notification 1: Pending -> Sent -> Read
(1, 1001, 0, DATEADD(day, -2, GETUTCDATE())),
(2, 1001, 1, DATEADD(day, -1, GETUTCDATE())),
(3, 1001, 2, DATEADD(hour, -12, GETUTCDATE())),

-- Notification 2: Pending -> Sent -> Read
(4, 1002, 0, DATEADD(day, -5, GETUTCDATE())),
(5, 1002, 1, DATEADD(day, -3, GETUTCDATE())),
(6, 1002, 2, DATEADD(day, -2, GETUTCDATE())),

-- Notification 3: Pending (only one status)
(7, 1003, 0, DATEADD(hour, -1, GETUTCDATE())),

-- Notification 4: Pending -> Sent -> Expired
(8, 1004, 0, DATEADD(day, -12, GETUTCDATE())),
(9, 1004, 1, DATEADD(day, -10, GETUTCDATE())),
(10, 1004, 3, DATEADD(day, -1, GETUTCDATE())),

-- Notification 5: Pending -> Failed
(11, 1005, 0, DATEADD(day, -2, GETUTCDATE())),
(12, 1005, 4, DATEADD(day, -1, GETUTCDATE())),

-- Notification 6: Pending -> Sent
(13, 1006, 0, DATEADD(minute, -30, GETUTCDATE())),
(14, 1006, 1, GETUTCDATE()),

-- Notification 7: Pending -> Read
(15, 1007, 0, DATEADD(day, -30, GETUTCDATE())),
(16, 1007, 2, DATEADD(day, -29, GETUTCDATE()));
GO

-- Verify the data
SELECT 'Notifications:' AS TableName;
GO

SELECT 
    NotificationId,
    CorrelationId,
    LearnerAccountId,
    Category,
    Heading,
    LEFT(Body, 50) + '...' AS BodyPreview,
    StatusId,
    NotificationTime,
    TimeToExpire,
    TimeReceived,
    Link,
    UrgencyId
FROM [dbo].[Notification] 
ORDER BY [NotificationTime] DESC;
GO

SELECT 'Status History:' AS TableName;
GO

SELECT * FROM [dbo].[StatusHistory] 
ORDER BY [ChangeDate] DESC;
GO