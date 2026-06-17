using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerNotifications.Application.Data;
using SFA.DAS.LearnerNotifications.Application.Services;

namespace SFA.DAS.LearnerNotifications.Application.Tests.Unit
{
    [TestFixture]
    public class NotificationProcessorTests
    {
        private Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
        }

        [Test]
        public async Task Stores_Notifications()
        {
            var dataContextMock = fixture.Freeze<Mock<ILearnerNotificationsDataContext>>();
            //fixture.Create<ILearnerNotificationsDataContext>();.Verify(x => x.SaveNotification(It.IsAny<Models.Notification>()));
            var processor = fixture.Create<NotificationProcessor>();
            var message = new Messages.Commands.SendNotification
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Category = "TestCategory",
                Heading = "Test Heading",
                Body = "Test Body",
                LinkUrl = "http://test.com",
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(7),
                Urgency = Messages.Commands.Urgency.Medium
            };
            await processor.Process(message);
            dataContextMock.Verify(x => x.SaveNotification(It.Is<SFA.DAS.LearnerNotifications.Models.Notification>(notification =>
                notification.Heading == message.Heading &&
                notification.Body == message.Body &&
                notification.LinkUrl == message.LinkUrl &&
                notification.Category == message.Category &&
                notification.LearnerAccountId == message.LearnerAccountId &&
                notification.CorrelationId == message.CorrelationId  &&
                notification.NotificationTime == message.NotificationTime

            )), Times.Once);
        }

        [Test]
        public async Task Stores_New_Notifications_With_Status_As_Unread()
        {
            var dataContextMock = fixture.Freeze<Mock<ILearnerNotificationsDataContext>>();
            //fixture.Create<ILearnerNotificationsDataContext>();.Verify(x => x.SaveNotification(It.IsAny<Models.Notification>()));
            var processor = fixture.Create<NotificationProcessor>();
            var message = new Messages.Commands.SendNotification
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Category = "TestCategory",
                Heading = "Test Heading",
                Body = "Test Body",
                LinkUrl = "http://test.com",
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(7),
                Urgency = Messages.Commands.Urgency.Medium
            };
            await processor.Process(message);
            dataContextMock.Verify(x => x.SaveNotification(It.Is<SFA.DAS.LearnerNotifications.Models.Notification>(notification =>
                notification.Status == SFA.DAS.LearnerNotifications.Models.NotificationStatus.Unread
            )), Times.Once);
        }

    }
}
