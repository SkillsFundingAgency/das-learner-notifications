using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.LearnerNotifications.Application.Commands;
using SFA.DAS.LearnerNotifications.Application.Models;
using SFA.DAS.LearnerNotifications.Application.Notifications;
using SFA.DAS.LearnerNotifications.Application.Queries;
using SFA.DAS.LearnerNotifications.Application.Queries.Results;
using SFA.DAS.LearnerNotifications.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using StatusEnum = SFA.DAS.LearnerNotifications.Application.Models.Status;

namespace SFA.DAS.LearnerNotifications.Application.UnitTests.DataFixture
{
    public class HandlerTests : LearnerNotificationsDbContextFixture
    {
        private readonly Fixture _fixture = new();
        private NotificationService _service = null!;

        [SetUp]
        public void Setup()
        {
            _service = new NotificationService(DbContext);
        }

        // ==================== GetNotificationsByAccount Tests ====================

        [Test, MoqAutoData]
        public async Task GetNotificationsByAccount_ReturnsNotifications_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            await PopulateDbContextWithNotifications(accountId);

            var query = new GetNotificationsByAccountIdentifierQuery
            {
                AccountIdentifier = accountId
            };

            // Act
            var result = await _service.GetNotificationsByAccountAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Notifications, Is.Not.Empty);
            Assert.That(result.Notifications.Count, Is.EqualTo(2));
        }

        [Test, MoqAutoData]
        public async Task GetNotificationsByAccount_ReturnsEmpty_WhenNoNotifications_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var query = new GetNotificationsByAccountIdentifierQuery
            {
                AccountIdentifier = accountId
            };

            // Act
            var result = await _service.GetNotificationsByAccountAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Notifications, Is.Empty);
        }

        [Test, MoqAutoData]
        public async Task GetNotificationsByAccount_OrdersByNotificationTimeDescending_Default_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notification1 = _fixture.Build<Notification>()
                .With(n => n.NotificationId, 1L)
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.NotificationTime, new DateTime(2023, 1, 1))
                .Create();
            var notification2 = _fixture.Build<Notification>()
                .With(n => n.NotificationId, 2L)
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.NotificationTime, new DateTime(2023, 1, 3))
                .Create();
            var notification3 = _fixture.Build<Notification>()
                .With(n => n.NotificationId, 3L)
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.NotificationTime, new DateTime(2023, 1, 2))
                .Create();
            await DbContext.Notifications.AddRangeAsync(notification1, notification2, notification3);
            await DbContext.SaveChangesAsync();

            var query = new GetNotificationsByAccountIdentifierQuery
            {
                AccountIdentifier = accountId,
                Order = SortOrder.Descending
            };

            // Act
            var result = await _service.GetNotificationsByAccountAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result.Notifications.Count, Is.EqualTo(3));
            Assert.That(result.Notifications[0].NotificationId, Is.EqualTo(2L));
            Assert.That(result.Notifications[1].NotificationId, Is.EqualTo(3L));
            Assert.That(result.Notifications[2].NotificationId, Is.EqualTo(1L));
        }

        [Test, MoqAutoData]
        public async Task GetNotificationsByAccount_OrdersByNotificationTimeAscending_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notification1 = _fixture.Build<Notification>()
                .With(n => n.NotificationId, 1L)
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.NotificationTime, new DateTime(2023, 1, 1))
                .Create();
            var notification2 = _fixture.Build<Notification>()
                .With(n => n.NotificationId, 2L)
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.NotificationTime, new DateTime(2023, 1, 3))
                .Create();
            var notification3 = _fixture.Build<Notification>()
                .With(n => n.NotificationId, 3L)
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.NotificationTime, new DateTime(2023, 1, 2))
                .Create();
            await DbContext.Notifications.AddRangeAsync(notification1, notification2, notification3);
            await DbContext.SaveChangesAsync();

            var query = new GetNotificationsByAccountIdentifierQuery
            {
                AccountIdentifier = accountId,
                Order = SortOrder.Ascending
            };

            // Act
            var result = await _service.GetNotificationsByAccountAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result.Notifications.Count, Is.EqualTo(3));
            Assert.That(result.Notifications[0].NotificationId, Is.EqualTo(1L));
            Assert.That(result.Notifications[1].NotificationId, Is.EqualTo(3L));
            Assert.That(result.Notifications[2].NotificationId, Is.EqualTo(2L));
        }

        [Test, MoqAutoData]
        public async Task GetNotificationsByAccount_FiltersByDateFrom_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var cutoffDate = new DateTime(2023, 6, 15);
            var older = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.NotificationTime, new DateTime(2023, 6, 10))
                .Create();
            var onCutoff = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.NotificationTime, cutoffDate)
                .Create();
            var newer = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.NotificationTime, new DateTime(2023, 6, 20))
                .Create();
            await DbContext.Notifications.AddRangeAsync(older, onCutoff, newer);
            await DbContext.SaveChangesAsync();

            var query = new GetNotificationsByAccountIdentifierQuery
            {
                AccountIdentifier = accountId,
                DateFrom = cutoffDate
            };

            // Act
            var result = await _service.GetNotificationsByAccountAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result.Notifications.Count, Is.EqualTo(2));
            Assert.That(result.Notifications.All(n => n.NotificationTime >= cutoffDate), Is.True);
        }

        [Test, MoqAutoData]
        public async Task GetNotificationsByAccount_FiltersBySingleStatus_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var unread = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte)StatusEnum.Unread)
                .Create();
            var acknowledged = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte)StatusEnum.Acknowledged)
                .Create();
            var hidden = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte)StatusEnum.Hidden)
                .Create();
            await DbContext.Notifications.AddRangeAsync(unread, acknowledged, hidden);
            await DbContext.SaveChangesAsync();

            var query = new GetNotificationsByAccountIdentifierQuery
            {
                AccountIdentifier = accountId,
                Statuses = new System.Collections.Generic.List<StatusEnum> { StatusEnum.Acknowledged }
            };

            // Act
            var result = await _service.GetNotificationsByAccountAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result.Notifications.Count, Is.EqualTo(1));
            Assert.That(result.Notifications[0].StatusId, Is.EqualTo((byte)StatusEnum.Acknowledged));
        }

        [Test, MoqAutoData]
        public async Task GetNotificationsByAccount_FiltersByMultipleStatuses_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var unread = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte)StatusEnum.Unread)
                .Create();
            var acknowledged = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte)StatusEnum.Acknowledged)
                .Create();
            var hidden = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte)StatusEnum.Hidden)
                .Create();
            var expired = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte)StatusEnum.Expired)
                .Create();
            await DbContext.Notifications.AddRangeAsync(unread, acknowledged, hidden, expired);
            await DbContext.SaveChangesAsync();

            var query = new GetNotificationsByAccountIdentifierQuery
            {
                AccountIdentifier = accountId,
                Statuses = new System.Collections.Generic.List<StatusEnum> { StatusEnum.Unread, StatusEnum.Expired }
            };

            // Act
            var result = await _service.GetNotificationsByAccountAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result.Notifications.Count, Is.EqualTo(2));
            Assert.That(result.Notifications.All(n => n.StatusId == (byte)StatusEnum.Unread || n.StatusId == (byte)StatusEnum.Expired), Is.True);
        }

        [Test, MoqAutoData]
        public async Task GetNotificationsByAccount_FiltersByStatusAndDateFromAndOrder_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var cutoff = new DateTime(2023, 5, 1);
            var unreadBefore = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte)StatusEnum.Unread)
                .With(n => n.NotificationTime, new DateTime(2023, 4, 25))
                .Create();
            var unreadAfterEarlier = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte)StatusEnum.Unread)
                .With(n => n.NotificationTime, new DateTime(2023, 5, 10))
                .Create();
            var unreadAfterLater = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte)StatusEnum.Unread)
                .With(n => n.NotificationTime, new DateTime(2023, 5, 20))
                .Create();
            var acknowledgedAfter = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte)StatusEnum.Acknowledged)
                .With(n => n.NotificationTime, new DateTime(2023, 5, 15))
                .Create();
            await DbContext.Notifications.AddRangeAsync(unreadBefore, unreadAfterEarlier, unreadAfterLater, acknowledgedAfter);
            await DbContext.SaveChangesAsync();

            var query = new GetNotificationsByAccountIdentifierQuery
            {
                AccountIdentifier = accountId,
                DateFrom = cutoff,
                Statuses = new System.Collections.Generic.List<StatusEnum> { StatusEnum.Unread },
                Order = SortOrder.Ascending
            };

            // Act
            var result = await _service.GetNotificationsByAccountAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result.Notifications.Count, Is.EqualTo(2));
            Assert.That(result.Notifications[0].NotificationTime, Is.EqualTo(new DateTime(2023, 5, 10)));
            Assert.That(result.Notifications[1].NotificationTime, Is.EqualTo(new DateTime(2023, 5, 20)));
        }

        [Test, MoqAutoData]
        public async Task GetNotificationsByAccount_WhenStatusesListEmpty_ReturnsAllNotifications_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            await PopulateDbContextWithNotifications(accountId);
            var query = new GetNotificationsByAccountIdentifierQuery
            {
                AccountIdentifier = accountId,
                Statuses = new System.Collections.Generic.List<StatusEnum>()
            };

            // Act
            var result = await _service.GetNotificationsByAccountAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result.Notifications.Count, Is.EqualTo(2));
        }

        [Test, MoqAutoData]
        public async Task GetNotificationsByAccount_WhenStatusesNull_ReturnsAllNotifications_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            await PopulateDbContextWithNotifications(accountId);
            var query = new GetNotificationsByAccountIdentifierQuery
            {
                AccountIdentifier = accountId,
                Statuses = null
            };

            // Act
            var result = await _service.GetNotificationsByAccountAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result.Notifications.Count, Is.EqualTo(2));
        }

        // ==================== GetNotificationById Tests ====================

        [Test, MoqAutoData]
        public async Task GetNotificationById_ReturnsNotification_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notificationId = 1L;
            await PopulateDbContextWithNotifications(accountId, notificationId);

            var query = new GetNotificationByIdQuery
            {
                AccountIdentifier = accountId,
                NotificationIdentifier = notificationId
            };

            // Act
            var result = await _service.GetNotificationByIdAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.NotificationId, Is.EqualTo(notificationId));
            Assert.That(result.LearnerAccountId, Is.EqualTo(accountId));
        }

        [Test, MoqAutoData]
        public async Task GetNotificationById_ReturnsNull_WhenNotFound_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notificationId = 999L;

            var query = new GetNotificationByIdQuery
            {
                AccountIdentifier = accountId,
                NotificationIdentifier = notificationId
            };

            // Act
            var result = await _service.GetNotificationByIdAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
        }

        // ==================== GetNotificationStatus Tests ====================

        [Test, MoqAutoData]
        public async Task GetNotificationStatus_ReturnsStatus_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notificationId = 1L;
            var statusId = (byte)2; // Acknowledged
            await PopulateDbContextWithNotificationsAndStatusHistory(accountId, notificationId, statusId);

            var query = new GetNotificationStatusQuery
            {
                AccountIdentifier = accountId,
                NotificationIdentifier = notificationId
            };

            // Act
            var result = await _service.GetNotificationStatusAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusId, Is.EqualTo(statusId));
            Assert.That(result.StatusName, Is.EqualTo("Acknowledged"));
            Assert.That(result.LastUpdated, Is.GreaterThan(DateTime.MinValue));
        }

        [Test, MoqAutoData]
        public async Task GetNotificationStatus_ReturnsNull_WhenNotificationNotFound_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notificationId = 999L;

            var query = new GetNotificationStatusQuery
            {
                AccountIdentifier = accountId,
                NotificationIdentifier = notificationId
            };

            // Act
            var result = await _service.GetNotificationStatusAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test, MoqAutoData]
        public async Task GetNotificationStatus_WhenNoStatusHistory_ReturnsDefaultLastUpdated()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notificationId = 1L;
            var statusId = (byte)1; // Unread
            await PopulateDbContextWithNotifications(accountId, notificationId, statusId);

            var query = new GetNotificationStatusQuery
            {
                AccountIdentifier = accountId,
                NotificationIdentifier = notificationId
            };

            // Act
            var result = await _service.GetNotificationStatusAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusId, Is.EqualTo(statusId));
            Assert.That(result.StatusName, Is.EqualTo("Unread"));
            Assert.That(result.LastUpdated, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(2)));
        }

        [Test, MoqAutoData]
        public async Task GetNotificationStatus_WhenStatusIdIsNull_UsesDefaultValue()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notificationId = 1L;
            var notification = _fixture.Build<Notification>()
                .With(n => n.NotificationId, notificationId)
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte?)null)
                .Create();
            await DbContext.Notifications.AddAsync(notification);
            await DbContext.SaveChangesAsync();

            var query = new GetNotificationStatusQuery
            {
                AccountIdentifier = accountId,
                NotificationIdentifier = notificationId
            };

            // Act
            var result = await _service.GetNotificationStatusAsync(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusId, Is.EqualTo(0));
            Assert.That(result.StatusName, Is.EqualTo("Unread"));
        }

        [Test, MoqAutoData]
        public async Task GetNotificationStatus_ReturnsCorrectNamesForAllStatuses_Test()
        {
            // Arrange
            var accountId1 = Guid.NewGuid();
            await PopulateDbContextWithNotifications(accountId1, 1L, 1);
            var query1 = new GetNotificationStatusQuery { AccountIdentifier = accountId1, NotificationIdentifier = 1L };
            var result1 = await _service.GetNotificationStatusAsync(query1, CancellationToken.None);
            Assert.That(result1.StatusName, Is.EqualTo("Unread"));

            var accountId2 = Guid.NewGuid();
            await PopulateDbContextWithNotifications(accountId2, 2L, 2);
            var query2 = new GetNotificationStatusQuery { AccountIdentifier = accountId2, NotificationIdentifier = 2L };
            var result2 = await _service.GetNotificationStatusAsync(query2, CancellationToken.None);
            Assert.That(result2.StatusName, Is.EqualTo("Acknowledged"));

            var accountId3 = Guid.NewGuid();
            await PopulateDbContextWithNotifications(accountId3, 3L, 3);
            var query3 = new GetNotificationStatusQuery { AccountIdentifier = accountId3, NotificationIdentifier = 3L };
            var result3 = await _service.GetNotificationStatusAsync(query3, CancellationToken.None);
            Assert.That(result3.StatusName, Is.EqualTo("Hidden"));

            var accountId4 = Guid.NewGuid();
            await PopulateDbContextWithNotifications(accountId4, 4L, 4);
            var query4 = new GetNotificationStatusQuery { AccountIdentifier = accountId4, NotificationIdentifier = 4L };
            var result4 = await _service.GetNotificationStatusAsync(query4, CancellationToken.None);
            Assert.That(result4.StatusName, Is.EqualTo("Expired"));

            var accountId5 = Guid.NewGuid();
            await PopulateDbContextWithNotifications(accountId5, 5L, 99);
            var query99 = new GetNotificationStatusQuery { AccountIdentifier = accountId5, NotificationIdentifier = 5L };
            var result99 = await _service.GetNotificationStatusAsync(query99, CancellationToken.None);
            Assert.That(result99.StatusName, Is.EqualTo("Unread"));
        }

        // ==================== SetNotificationStatus Tests ====================

        [Test, MoqAutoData]
        public async Task SetNotificationStatus_UpdatesStatus_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notificationId = 1L;
            var initialStatus = (byte)1; // Unread
            var newStatus = 2; // Acknowledged

            await PopulateDbContextWithNotifications(accountId, notificationId, initialStatus);

            var command = new SetNotificationStatusCommand
            {
                AccountIdentifier = accountId,
                NotificationIdentifier = notificationId,
                StatusId = newStatus
            };

            // Act
            await _service.SetNotificationStatusAsync(command, CancellationToken.None);

            // Assert
            var updatedNotification = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.LearnerAccountId == accountId);
            Assert.That(updatedNotification, Is.Not.Null);
            Assert.That(updatedNotification.StatusId, Is.EqualTo((byte)newStatus));

            var statusHistory = await DbContext.StatusHistory
                .Where(sh => sh.NotificationId == notificationId)
                .OrderByDescending(sh => sh.ChangeDate)
                .FirstOrDefaultAsync();
            Assert.That(statusHistory, Is.Not.Null);
            Assert.That(statusHistory.Status, Is.EqualTo((byte)newStatus));
            Assert.That(statusHistory.ChangeDate, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-5)));
        }

        [Test, MoqAutoData]
        public async Task SetNotificationStatus_DoesNothing_WhenNotificationNotFound_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notificationId = 999L;

            var command = new SetNotificationStatusCommand
            {
                AccountIdentifier = accountId,
                NotificationIdentifier = notificationId,
                StatusId = 2
            };

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await _service.SetNotificationStatusAsync(command, CancellationToken.None));
        }

        // ==================== CreateNotification Tests ====================

        [Test, MoqAutoData]
        public async Task CreateNotification_CreatesNotification_Test()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var learnerAccountId = Guid.NewGuid();
            var notificationTime = DateTime.UtcNow.AddHours(-1);
            var timeToExpire = DateTime.UtcNow.AddDays(30);
            var timeReceived = DateTime.UtcNow.AddHours(-1.5);

            var command = new CreateNotificationCommand
            {
                CorrelationId = correlationId,
                LearnerAccountId = learnerAccountId,
                Category = "SystemAlert",
                Heading = "Important Update",
                Body = "Your account has been updated with new features.",
                StatusId = 1,
                NotificationTime = notificationTime,
                TimeToExpire = timeToExpire,
                TimeReceived = timeReceived,
                Link = "/account/settings",
                Urgency = 1
            };

            // Act
            await _service.CreateNotificationAsync(command, CancellationToken.None);

            // Assert
            var createdNotification = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.CorrelationId == correlationId && n.LearnerAccountId == learnerAccountId);
            Assert.That(createdNotification, Is.Not.Null);
            Assert.That(createdNotification.CorrelationId, Is.EqualTo(correlationId));
            Assert.That(createdNotification.LearnerAccountId, Is.EqualTo(learnerAccountId));
            Assert.That(createdNotification.Category, Is.EqualTo("SystemAlert"));
            Assert.That(createdNotification.Heading, Is.EqualTo("Important Update"));
            Assert.That(createdNotification.Body, Is.EqualTo("Your account has been updated with new features."));
            Assert.That(createdNotification.StatusId, Is.EqualTo(1));
            Assert.That(createdNotification.NotificationTime, Is.EqualTo(notificationTime));
            Assert.That(createdNotification.TimeToExpire, Is.EqualTo(timeToExpire));
            Assert.That(createdNotification.TimeReceived, Is.EqualTo(timeReceived));
        }

        [Test, MoqAutoData]
        public async Task CreateNotification_CreatesNotification_WithNullLink_Test()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var learnerAccountId = Guid.NewGuid();

            var command = new CreateNotificationCommand
            {
                CorrelationId = correlationId,
                LearnerAccountId = learnerAccountId,
                Category = "TrainingReminder",
                Heading = "Training Due",
                Body = "Please complete your training.",
                StatusId = 1,
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(7),
                TimeReceived = DateTime.UtcNow,
                Link = null,
                Urgency = 0
            };

            // Act
            await _service.CreateNotificationAsync(command, CancellationToken.None);

            // Assert
            var createdNotification = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.CorrelationId == correlationId);
            Assert.That(createdNotification, Is.Not.Null);
            Assert.That(createdNotification.StatusId, Is.EqualTo(1));
            Assert.That(createdNotification.LearnerAccountId, Is.EqualTo(learnerAccountId));
        }

        [Test, MoqAutoData]
        public async Task CreateNotification_CreatesMultipleNotifications_ForSameLearner_Test()
        {
            // Arrange
            var learnerAccountId = Guid.NewGuid();

            var command1 = new CreateNotificationCommand
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = learnerAccountId,
                Category = "SystemAlert",
                Heading = "First Notification",
                Body = "First notification body",
                StatusId = 1,
                NotificationTime = DateTime.UtcNow.AddHours(-2),
                TimeToExpire = DateTime.UtcNow.AddDays(1),
                TimeReceived = DateTime.UtcNow.AddHours(-2),
                Link = "/first",
                Urgency = 1
            };

            var command2 = new CreateNotificationCommand
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = learnerAccountId,
                Category = "Training",
                Heading = "Second Notification",
                Body = "Second notification body",
                StatusId = 2,
                NotificationTime = DateTime.UtcNow.AddHours(-1),
                TimeToExpire = DateTime.UtcNow.AddDays(2),
                TimeReceived = DateTime.UtcNow.AddHours(-1),
                Link = "/second",
                Urgency = 2
            };

            // Act
            await _service.CreateNotificationAsync(command1, CancellationToken.None);
            await _service.CreateNotificationAsync(command2, CancellationToken.None);

            // Assert
            var notifications = await DbContext.Notifications
                .Where(n => n.LearnerAccountId == learnerAccountId)
                .ToListAsync();
            Assert.That(notifications, Has.Count.EqualTo(2));
            Assert.That(notifications[0].Heading, Is.EqualTo("First Notification"));
            Assert.That(notifications[1].Heading, Is.EqualTo("Second Notification"));
        }

        [Test, MoqAutoData]
        public async Task CreateNotification_CreatesNotifications_ForDifferentLearners_Test()
        {
            // Arrange
            var learnerAccountId1 = Guid.NewGuid();
            var learnerAccountId2 = Guid.NewGuid();

            var command1 = new CreateNotificationCommand
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = learnerAccountId1,
                Category = "Alert",
                Heading = "Notification for Learner 1",
                Body = "Body for learner 1",
                StatusId = 1,
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(1),
                TimeReceived = DateTime.UtcNow,
                Link = "/learner1",
                Urgency = 1
            };

            var command2 = new CreateNotificationCommand
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = learnerAccountId2,
                Category = "Alert",
                Heading = "Notification for Learner 2",
                Body = "Body for learner 2",
                StatusId = 2,
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(1),
                TimeReceived = DateTime.UtcNow,
                Link = "/learner2",
                Urgency = 2
            };

            // Act
            await _service.CreateNotificationAsync(command1, CancellationToken.None);
            await _service.CreateNotificationAsync(command2, CancellationToken.None);

            // Assert
            var notification1 = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.LearnerAccountId == learnerAccountId1);
            var notification2 = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.LearnerAccountId == learnerAccountId2);
            Assert.That(notification1, Is.Not.Null);
            Assert.That(notification2, Is.Not.Null);
            Assert.That(notification1.LearnerAccountId, Is.EqualTo(learnerAccountId1));
            Assert.That(notification2.LearnerAccountId, Is.EqualTo(learnerAccountId2));
            Assert.That(notification1.Heading, Is.EqualTo("Notification for Learner 1"));
            Assert.That(notification2.Heading, Is.EqualTo("Notification for Learner 2"));
        }

        [Test, MoqAutoData]
        public async Task CreateNotification_CreatesNotification_WithDifferentStatusIds_Test()
        {
            // Arrange
            var commandUnread = new CreateNotificationCommand
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Category = "Test",
                Heading = "Unread Notification",
                Body = "This is unread",
                StatusId = 1,
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(1),
                TimeReceived = DateTime.UtcNow,
                Link = "/unread",
                Urgency = 0
            };

            var commandAcknowledged = new CreateNotificationCommand
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Category = "Test",
                Heading = "Acknowledged Notification",
                Body = "This is acknowledged",
                StatusId = 2,
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(1),
                TimeReceived = DateTime.UtcNow,
                Link = "/acknowledged",
                Urgency = 1
            };

            // Act
            await _service.CreateNotificationAsync(commandUnread, CancellationToken.None);
            await _service.CreateNotificationAsync(commandAcknowledged, CancellationToken.None);

            // Assert
            var unreadNotification = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.CorrelationId == commandUnread.CorrelationId);
            var acknowledgedNotification = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.CorrelationId == commandAcknowledged.CorrelationId);
            Assert.That(unreadNotification.StatusId, Is.EqualTo(1));
            Assert.That(acknowledgedNotification.StatusId, Is.EqualTo(2));
        }

        [Test, MoqAutoData]
        public async Task CreateNotification_CreatesNotification_WithDifferentUrgencyLevels_Test()
        {
            // Arrange
            var commandLow = new CreateNotificationCommand
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Category = "Low",
                Heading = "Low Urgency",
                Body = "Low urgency notification",
                StatusId = 1,
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(7),
                TimeReceived = DateTime.UtcNow,
                Link = "/low",
                Urgency = 0
            };

            var commandHigh = new CreateNotificationCommand
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Category = "High",
                Heading = "High Urgency",
                Body = "High urgency notification",
                StatusId = 2,
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(1),
                TimeReceived = DateTime.UtcNow,
                Link = "/high",
                Urgency = 2
            };

            // Act
            await _service.CreateNotificationAsync(commandLow, CancellationToken.None);
            await _service.CreateNotificationAsync(commandHigh, CancellationToken.None);

            // Assert
            var lowNotification = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.CorrelationId == commandLow.CorrelationId);
            var highNotification = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.CorrelationId == commandHigh.CorrelationId);
            Assert.That(lowNotification, Is.Not.Null);
            Assert.That(highNotification, Is.Not.Null);
        }

        // ==================== DeleteNotification Tests ====================

        [Test, MoqAutoData]
        public async Task DeleteNotification_DeletesNotification_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notificationId = 1L;
            await PopulateDbContextWithNotificationsAndStatusHistory(accountId, notificationId, 1);

            var command = new DeleteNotificationCommand
            {
                AccountIdentifier = accountId,
                NotificationIdentifier = notificationId
            };

            var notificationBefore = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.LearnerAccountId == accountId);
            Assert.That(notificationBefore, Is.Not.Null);
            var statusHistoryBefore = await DbContext.StatusHistory
                .Where(sh => sh.NotificationId == notificationId).ToListAsync();
            Assert.That(statusHistoryBefore, Is.Not.Empty);

            // Act
            await _service.DeleteNotificationAsync(command, CancellationToken.None);

            // Assert
            var notificationAfter = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.LearnerAccountId == accountId);
            Assert.That(notificationAfter, Is.Null);
            var statusHistoryAfter = await DbContext.StatusHistory
                .Where(sh => sh.NotificationId == notificationId).ToListAsync();
            Assert.That(statusHistoryAfter, Is.Empty);
        }

        [Test, MoqAutoData]
        public async Task DeleteNotification_DeletesOnlySpecifiedNotification_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notificationId1 = 1L;
            var notificationId2 = 2L;
            await PopulateDbContextWithNotifications(accountId, notificationId1, 1);
            await PopulateDbContextWithNotifications(accountId, notificationId2, 2);

            var command = new DeleteNotificationCommand
            {
                AccountIdentifier = accountId,
                NotificationIdentifier = notificationId1
            };

            // Act
            await _service.DeleteNotificationAsync(command, CancellationToken.None);

            // Assert
            var deleted = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId1 && n.LearnerAccountId == accountId);
            var remaining = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId2 && n.LearnerAccountId == accountId);
            Assert.That(deleted, Is.Null);
            Assert.That(remaining, Is.Not.Null);
            Assert.That(remaining.NotificationId, Is.EqualTo(notificationId2));
        }

        [Test, MoqAutoData]
        public async Task DeleteNotification_DoesNothing_WhenNotificationNotFound_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notificationId = 999L;
            var command = new DeleteNotificationCommand
            {
                AccountIdentifier = accountId,
                NotificationIdentifier = notificationId
            };

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await _service.DeleteNotificationAsync(command, CancellationToken.None));
        }

        [Test, MoqAutoData]
        public async Task DeleteNotification_DoesNotDelete_WhenAccountIdentifierDoesNotMatch_Test()
        {
            // Arrange
            var accountId1 = Guid.NewGuid();
            var accountId2 = Guid.NewGuid();
            var notificationId = 1L;
            await PopulateDbContextWithNotifications(accountId1, notificationId, 1);

            var command = new DeleteNotificationCommand
            {
                AccountIdentifier = accountId2,
                NotificationIdentifier = notificationId
            };

            // Act
            await _service.DeleteNotificationAsync(command, CancellationToken.None);

            // Assert
            var notification = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.LearnerAccountId == accountId1);
            Assert.That(notification, Is.Not.Null);
        }

        [Test, MoqAutoData]
        public async Task DeleteNotification_DeletesNotificationAndStatusHistory_Test()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var notificationId = 1L;
            var notification = _fixture.Build<Notification>()
                .With(n => n.NotificationId, notificationId)
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, (byte)2)
                .Create();
            var statusHistory1 = _fixture.Build<StatusHistory>()
                .With(sh => sh.StatusHistoryId, 1L)
                .With(sh => sh.NotificationId, notificationId)
                .With(sh => sh.Status, (byte)1)
                .With(sh => sh.ChangeDate, DateTime.UtcNow.AddHours(-2))
                .Create();
            var statusHistory2 = _fixture.Build<StatusHistory>()
                .With(sh => sh.StatusHistoryId, 2L)
                .With(sh => sh.NotificationId, notificationId)
                .With(sh => sh.Status, (byte)2)
                .With(sh => sh.ChangeDate, DateTime.UtcNow.AddHours(-1))
                .Create();
            var statusHistory3 = _fixture.Build<StatusHistory>()
                .With(sh => sh.StatusHistoryId, 3L)
                .With(sh => sh.NotificationId, notificationId)
                .With(sh => sh.Status, (byte)3)
                .With(sh => sh.ChangeDate, DateTime.UtcNow)
                .Create();
            await DbContext.Notifications.AddAsync(notification);
            await DbContext.StatusHistory.AddRangeAsync(statusHistory1, statusHistory2, statusHistory3);
            await DbContext.SaveChangesAsync();

            var command = new DeleteNotificationCommand
            {
                AccountIdentifier = accountId,
                NotificationIdentifier = notificationId
            };

            // Act
            await _service.DeleteNotificationAsync(command, CancellationToken.None);

            // Assert
            var deletedNotification = await DbContext.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);
            var remainingHistory = await DbContext.StatusHistory
                .Where(sh => sh.NotificationId == notificationId).ToListAsync();
            Assert.That(deletedNotification, Is.Null);
            Assert.That(remainingHistory, Is.Empty);
        }

        // ------------------------------------------------------------------
        // Private helper methods
        // ------------------------------------------------------------------

        private async Task PopulateDbContextWithNotifications(Guid accountId, long? notificationId = null, byte? statusId = null)
        {
            var notification = _fixture.Build<Notification>()
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, statusId ?? 1)
                .Create();

            if (notificationId.HasValue)
                notification.NotificationId = notificationId.Value;

            if (notificationId == null)
            {
                var secondNotification = _fixture.Build<Notification>()
                    .With(n => n.LearnerAccountId, accountId)
                    .With(n => n.StatusId, statusId ?? 2)
                    .Create();
                await DbContext.Notifications.AddAsync(secondNotification);
            }

            await DbContext.Notifications.AddAsync(notification);
            await DbContext.SaveChangesAsync();
        }

        private async Task PopulateDbContextWithNotificationsAndStatusHistory(Guid accountId, long notificationId, byte statusId)
        {
            var notification = _fixture.Build<Notification>()
                .With(n => n.NotificationId, notificationId)
                .With(n => n.LearnerAccountId, accountId)
                .With(n => n.StatusId, statusId)
                .Create();

            var statusHistory = _fixture.Build<StatusHistory>()
                .With(sh => sh.NotificationId, notificationId)
                .With(sh => sh.Status, statusId)
                .With(sh => sh.ChangeDate, DateTime.UtcNow.AddHours(-1))
                .Create();

            await DbContext.Notifications.AddAsync(notification);
            await DbContext.StatusHistory.AddAsync(statusHistory);
            await DbContext.SaveChangesAsync();
        }
    }
}
