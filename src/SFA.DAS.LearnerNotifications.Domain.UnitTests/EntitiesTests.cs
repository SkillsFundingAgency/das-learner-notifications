using System;
using NUnit.Framework;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Domain.UnitTests
{
    public class EntitiesTests
    {
        [Test]
        public void Notification_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var notification = new Notification();

            // Assert
            Assert.That(notification.NotificationId, Is.EqualTo(0));
            Assert.That(notification.CorrelationId, Is.Null);
            Assert.That(notification.LearnerAccountId, Is.Null);
            Assert.That(notification.Category, Is.Null);
            Assert.That(notification.Heading, Is.Null);
            Assert.That(notification.Body, Is.Null);
            Assert.That(notification.StatusId, Is.Null);
            Assert.That(notification.NotificationTime, Is.Null);
            Assert.That(notification.TimeToExpire, Is.Null);
            Assert.That(notification.TimeReceived, Is.Null);
            Assert.That(notification.Link, Is.Null);
            Assert.That(notification.UrgencyId, Is.Null);
        }

        [Test]
        public void Notification_Properties_CanBeSetAndRetrieved()
        {
            // Arrange
            var notification = new Notification();
            var now = DateTime.UtcNow;
            var correlationId = Guid.NewGuid();
            var learnerAccountId = Guid.NewGuid();

            // Act
            notification.NotificationId = 123;
            notification.CorrelationId = correlationId;
            notification.LearnerAccountId = learnerAccountId;
            notification.Category = "TestCategory";
            notification.Heading = "TestHeading";
            notification.Body = "TestBody";
            notification.StatusId = 1;
            notification.NotificationTime = now;
            notification.TimeToExpire = now.AddDays(7);
            notification.TimeReceived = now;
            notification.Link = "https://example.com";
            notification.UrgencyId = 2;

            // Assert
            Assert.That(notification.NotificationId, Is.EqualTo(123));
            Assert.That(notification.CorrelationId, Is.EqualTo(correlationId));
            Assert.That(notification.LearnerAccountId, Is.EqualTo(learnerAccountId));
            Assert.That(notification.Category, Is.EqualTo("TestCategory"));
            Assert.That(notification.Heading, Is.EqualTo("TestHeading"));
            Assert.That(notification.Body, Is.EqualTo("TestBody"));
            Assert.That(notification.StatusId, Is.EqualTo(1));
            Assert.That(notification.NotificationTime, Is.EqualTo(now));
            Assert.That(notification.TimeToExpire, Is.EqualTo(now.AddDays(7)));
            Assert.That(notification.TimeReceived, Is.EqualTo(now));
            Assert.That(notification.Link, Is.EqualTo("https://example.com"));
            Assert.That(notification.UrgencyId, Is.EqualTo(2));
        }

        [Test]
        public void Notification_NullableProperties_CanBeSetToNull()
        {
            // Arrange
            var notification = new Notification
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Category = "SomeCategory",
                Heading = "SomeHeading",
                Body = "SomeBody",
                StatusId = 2,
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(1),
                TimeReceived = DateTime.UtcNow,
                Link = "https://example.com",
                UrgencyId = 1
            };

            // Act
            notification.CorrelationId = null;
            notification.LearnerAccountId = null;
            notification.Category = null;
            notification.Heading = null;
            notification.Body = null;
            notification.StatusId = null;
            notification.NotificationTime = null;
            notification.TimeToExpire = null;
            notification.TimeReceived = null;
            notification.Link = null;
            notification.UrgencyId = null;

            // Assert
            Assert.That(notification.CorrelationId, Is.Null);
            Assert.That(notification.LearnerAccountId, Is.Null);
            Assert.That(notification.Category, Is.Null);
            Assert.That(notification.Heading, Is.Null);
            Assert.That(notification.Body, Is.Null);
            Assert.That(notification.StatusId, Is.Null);
            Assert.That(notification.NotificationTime, Is.Null);
            Assert.That(notification.TimeToExpire, Is.Null);
            Assert.That(notification.TimeReceived, Is.Null);
            Assert.That(notification.Link, Is.Null);
            Assert.That(notification.UrgencyId, Is.Null);
        }

        [Test]
        public void StatusHistory_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var statusHistory = new StatusHistory();

            // Assert
            Assert.That(statusHistory.StatusHistoryId, Is.EqualTo(0));
            Assert.That(statusHistory.NotificationId, Is.Null);
            Assert.That(statusHistory.Status, Is.Null);
            Assert.That(statusHistory.ChangeDate, Is.Null);
        }

        [Test]
        public void StatusHistory_Properties_CanBeSetAndRetrieved()
        {
            // Arrange
            var statusHistory = new StatusHistory();
            var changeDate = DateTime.UtcNow;

            // Act
            statusHistory.StatusHistoryId = 456;
            statusHistory.NotificationId = 123;
            statusHistory.Status = 2;
            statusHistory.ChangeDate = changeDate;

            // Assert
            Assert.That(statusHistory.StatusHistoryId, Is.EqualTo(456));
            Assert.That(statusHistory.NotificationId, Is.EqualTo(123));
            Assert.That(statusHistory.Status, Is.EqualTo(2));
            Assert.That(statusHistory.ChangeDate, Is.EqualTo(changeDate));
        }

        [Test]
        public void StatusHistory_NullableProperties_CanBeSetToNull()
        {
            // Arrange
            var statusHistory = new StatusHistory
            {
                NotificationId = 123,
                Status = 2,
                ChangeDate = DateTime.UtcNow
            };

            // Act
            statusHistory.NotificationId = null;
            statusHistory.Status = null;
            statusHistory.ChangeDate = null;

            // Assert
            Assert.That(statusHistory.NotificationId, Is.Null);
            Assert.That(statusHistory.Status, Is.Null);
            Assert.That(statusHistory.ChangeDate, Is.Null);
        }

        [Test]
        public void Status_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var status = new Status();

            // Assert
            Assert.That(status.Id, Is.EqualTo(0));
            Assert.That(status.Description, Is.Null);
        }

        [Test]
        public void Status_Properties_CanBeSetAndRetrieved()
        {
            // Arrange
            var status = new Status();

            // Act
            status.Id = 1;
            status.Description = "Unread";

            // Assert
            Assert.That(status.Id, Is.EqualTo(1));
            Assert.That(status.Description, Is.EqualTo("Unread"));
        }

        [Test]
        public void Status_Properties_CanBeSetToDifferentValues()
        {
            // Arrange
            var status1 = new Status();
            var status2 = new Status();
            var status3 = new Status();
            var status4 = new Status();

            // Act
            status1.Id = 1;
            status1.Description = "Unread";

            status2.Id = 2;
            status2.Description = "Acknowledged";

            status3.Id = 3;
            status3.Description = "Hidden";

            status4.Id = 4;
            status4.Description = "Expired";

            // Assert
            Assert.That(status1.Id, Is.EqualTo(1));
            Assert.That(status1.Description, Is.EqualTo("Unread"));

            Assert.That(status2.Id, Is.EqualTo(2));
            Assert.That(status2.Description, Is.EqualTo("Acknowledged"));

            Assert.That(status3.Id, Is.EqualTo(3));
            Assert.That(status3.Description, Is.EqualTo("Hidden"));

            Assert.That(status4.Id, Is.EqualTo(4));
            Assert.That(status4.Description, Is.EqualTo("Expired"));
        }

        [Test]
        public void Status_Description_CanBeLongString()
        {
            // Arrange
            var status = new Status();
            var longDescription = "This is a very long description that might be used to provide more detailed information about the status of a notification";

            // Act
            status.Id = 5;
            status.Description = longDescription;

            // Assert
            Assert.That(status.Id, Is.EqualTo(5));
            Assert.That(status.Description, Is.EqualTo(longDescription));
            Assert.That(status.Description.Length, Is.GreaterThan(50));
        }

        [Test]
        public void Status_Description_CanBeEmptyString()
        {
            // Arrange
            var status = new Status();

            // Act
            status.Id = 99;
            status.Description = string.Empty;

            // Assert
            Assert.That(status.Id, Is.EqualTo(99));
            Assert.That(status.Description, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Status_AllPossibleByteValues_CanBeSet()
        {
            // Arrange & Act
            var status = new Status();

            // Test minimum byte value
            status.Id = 0;
            Assert.That(status.Id, Is.EqualTo(0));

            // Test maximum byte value
            status.Id = 255;
            Assert.That(status.Id, Is.EqualTo(255));

            // Test some values in between
            status.Id = 10;
            Assert.That(status.Id, Is.EqualTo(10));

            status.Id = 128;
            Assert.That(status.Id, Is.EqualTo(128));
        }

        // ==================== New Urgency Tests ====================

        [Test]
        public void Urgency_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var urgency = new Urgency();

            // Assert
            Assert.That(urgency.Id, Is.EqualTo(0));
            Assert.That(urgency.Description, Is.Null);
        }

        [Test]
        public void Urgency_Properties_CanBeSetAndRetrieved()
        {
            // Arrange
            var urgency = new Urgency();

            // Act
            urgency.Id = 1;
            urgency.Description = "Medium";

            // Assert
            Assert.That(urgency.Id, Is.EqualTo(1));
            Assert.That(urgency.Description, Is.EqualTo("Medium"));
        }

        [Test]
        public void Urgency_Properties_CanBeSetToDifferentValues()
        {
            // Arrange
            var low = new Urgency();
            var medium = new Urgency();
            var high = new Urgency();

            // Act
            low.Id = 0;
            low.Description = "Low";

            medium.Id = 1;
            medium.Description = "Medium";

            high.Id = 2;
            high.Description = "High";

            // Assert
            Assert.That(low.Id, Is.EqualTo(0));
            Assert.That(low.Description, Is.EqualTo("Low"));

            Assert.That(medium.Id, Is.EqualTo(1));
            Assert.That(medium.Description, Is.EqualTo("Medium"));

            Assert.That(high.Id, Is.EqualTo(2));
            Assert.That(high.Description, Is.EqualTo("High"));
        }

        [Test]
        public void Urgency_Description_CanBeLongString()
        {
            // Arrange
            var urgency = new Urgency();
            var longDescription = "This urgency level indicates a very high priority requiring immediate action";

            // Act
            urgency.Id = 10;
            urgency.Description = longDescription;

            // Assert
            Assert.That(urgency.Id, Is.EqualTo(10));
            Assert.That(urgency.Description, Is.EqualTo(longDescription));
            Assert.That(urgency.Description.Length, Is.GreaterThan(30));
        }

        [Test]
        public void Urgency_Description_CanBeEmptyString()
        {
            // Arrange
            var urgency = new Urgency();

            // Act
            urgency.Id = 99;
            urgency.Description = string.Empty;

            // Assert
            Assert.That(urgency.Id, Is.EqualTo(99));
            Assert.That(urgency.Description, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Urgency_AllPossibleByteValues_CanBeSet()
        {
            // Arrange & Act
            var urgency = new Urgency();

            // Test minimum byte value
            urgency.Id = 0;
            Assert.That(urgency.Id, Is.EqualTo(0));

            // Test maximum byte value
            urgency.Id = 255;
            Assert.That(urgency.Id, Is.EqualTo(255));

            // Test some values in between
            urgency.Id = 10;
            Assert.That(urgency.Id, Is.EqualTo(10));

            urgency.Id = 128;
            Assert.That(urgency.Id, Is.EqualTo(128));
        }
    }
}
