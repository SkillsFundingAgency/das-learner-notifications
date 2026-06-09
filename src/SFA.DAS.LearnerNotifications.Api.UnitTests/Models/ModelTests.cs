using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.LearnerNotifications.Api.Controllers;
using SFA.DAS.LearnerNotifications.Application.Queries.Results;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.UnitTests
{
    [TestFixture]
    public class NotificationModelTests
    {
        [Test]
        public void NotificationEntity_Test()
        {
            var correlationId = Guid.NewGuid();
            var learnerAccountId = Guid.NewGuid();
            var notificationTime = new DateTime(2023, 1, 15, 10, 30, 0);
            var timeToExpire = new DateTime(2023, 1, 22, 10, 30, 0);
            var timeReceived = new DateTime(2023, 1, 15, 10, 0, 0);

            var sut = new Notification
            {
                NotificationId = 1234567890L,
                CorrelationId = correlationId,
                LearnerAccountId = learnerAccountId,
                Category = "SystemAlert",
                Heading = "Important Update",
                Body = "Your account has been updated with new features.",
                StatusId = 1,
                NotificationTime = notificationTime,
                TimeToExpire = timeToExpire,
                TimeReceived = timeReceived,
                UrgencyId = 2
            };

            Assert.AreEqual(1234567890L, sut.NotificationId);
            Assert.AreEqual(correlationId, sut.CorrelationId);
            Assert.AreEqual(learnerAccountId, sut.LearnerAccountId);
            Assert.AreEqual("SystemAlert", sut.Category);
            Assert.AreEqual("Important Update", sut.Heading);
            Assert.AreEqual("Your account has been updated with new features.", sut.Body);
            Assert.AreEqual(1, sut.StatusId);
            Assert.AreEqual(notificationTime, sut.NotificationTime);
            Assert.AreEqual(timeToExpire, sut.TimeToExpire);
            Assert.AreEqual(timeReceived, sut.TimeReceived);
            Assert.AreEqual(2, sut.UrgencyId);
        }

        [Test]
        public void StatusHistoryEntity_Test()
        {
            var changeDate = new DateTime(2023, 1, 15, 10, 35, 0);

            var sut = new StatusHistory
            {
                StatusHistoryId = 9876543210L,
                NotificationId = 1234567890L,
                Status = 2,
                ChangeDate = changeDate
            };

            Assert.AreEqual(9876543210L, sut.StatusHistoryId);
            Assert.AreEqual(1234567890L, sut.NotificationId);
            Assert.AreEqual(2, sut.Status);
            Assert.AreEqual(changeDate, sut.ChangeDate);
        }

        [Test]
        public void SetNotificationStatusRequest_Test()
        {
            var sut = new LearnerNotificationsController.SetNotificationStatusRequest
            {
                StatusId = 2
            };

            Assert.AreEqual(2, sut.StatusId);
        }

        [Test]
        public void GetNotificationsByAccountIdentifierResult_Test()
        {
            var notification1 = new Notification
            {
                NotificationId = 1,
                Heading = "First Notification",
                StatusId = 1
            };

            var notification2 = new Notification
            {
                NotificationId = 2,
                Heading = "Second Notification",
                StatusId = 2
            };

            var sut = new GetNotificationsByAccountIdentifierResult
            {
                Notifications = new List<Notification> { notification1, notification2 }
            };

            Assert.AreEqual(2, sut.Notifications.Count);
            Assert.AreEqual(1, sut.Notifications[0].NotificationId);
            Assert.AreEqual("First Notification", sut.Notifications[0].Heading);
            Assert.AreEqual(2, sut.Notifications[1].NotificationId);
            Assert.AreEqual("Second Notification", sut.Notifications[1].Heading);
        }

        [Test]
        public void GetNotificationStatusResult_Test()
        {
            var lastUpdated = new DateTime(2023, 1, 15, 10, 45, 0);

            var sut = new GetNotificationStatusResult
            {
                StatusId = 2,
                StatusName = "Read",
                LastUpdated = lastUpdated
            };

            Assert.AreEqual(2, sut.StatusId);
            Assert.AreEqual("Read", sut.StatusName);
            Assert.AreEqual(lastUpdated, sut.LastUpdated);
        }

        [Test]
        public void GetNotificationsByAccountIdentifierResult_EmptyList_Test()
        {
            var sut = new GetNotificationsByAccountIdentifierResult
            {
                Notifications = new List<Notification>()
            };

            Assert.AreEqual(0, sut.Notifications.Count);
        }

        [Test]
        public void NotificationEntity_NullableProperties_Test()
        {
            var sut = new Notification
            {
                NotificationId = 1,
                CorrelationId = null,
                LearnerAccountId = Guid.NewGuid(),
                Category = null,
                Heading = "Test",
                Body = null,
                StatusId = null,
                NotificationTime = null,
                TimeToExpire = null,
                TimeReceived = null,
                UrgencyId = null
            };

            Assert.AreEqual(1, sut.NotificationId);
            Assert.IsNull(sut.CorrelationId);
            Assert.IsNull(sut.Category);
            Assert.IsNull(sut.Body);
            Assert.IsNull(sut.StatusId);
            Assert.IsNull(sut.NotificationTime);
            Assert.IsNull(sut.TimeToExpire);
            Assert.IsNull(sut.TimeReceived);
            Assert.IsNull(sut.UrgencyId);
        }

        [Test]
        public void StatusHistoryEntity_NullableProperties_Test()
        {
            var sut = new StatusHistory
            {
                StatusHistoryId = 1,
                NotificationId = null,
                Status = null,
                ChangeDate = null
            };

            Assert.AreEqual(1, sut.StatusHistoryId);
            Assert.IsNull(sut.NotificationId);
            Assert.IsNull(sut.Status);
            Assert.IsNull(sut.ChangeDate);
        }
    }
}
