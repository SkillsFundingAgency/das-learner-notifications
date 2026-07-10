using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerNotifications.Application.Commands;
using SFA.DAS.LearnerNotifications.Application.Notifications;
using SFA.DAS.LearnerNotifications.LearnerNotificationService.Functions;
using SFA.DAS.LearnerNotifications.Messages.Commands;

namespace SFA.DAS.LearnerNotifications.LearnerNotificationService.UnitTests.Tests
{
    [TestFixture]
    public class HandleSendNotificationTests
    {
        private Mock<INotificationService> _notificationServiceMock;
        private Mock<ILogger<HandleSendNotification>> _sendLoggerMock;
        private HandleSendNotification _sendFunction;

        [SetUp]
        public void SetUp()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _sendLoggerMock = new Mock<ILogger<HandleSendNotification>>();
            _sendFunction = new HandleSendNotification(_notificationServiceMock.Object, _sendLoggerMock.Object);
        }

        [Test]
        public async Task HandleSendNotification_WithValidMessage_CallsCreateNotificationAsyncWithCorrectMapping()
        {
            var message = new SendNotification
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Category = "TestCategory",
                Heading = "Test Heading",
                Body = "Test Body",
                LinkUrl = "http://test.com",
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(7),
                Urgency = Urgency.Medium
            };

            await _sendFunction.Run(message, CancellationToken.None);

            _notificationServiceMock.Verify(s => s.CreateNotificationAsync(
                It.Is<CreateNotificationCommand>(cmd =>
                    cmd.CorrelationId == message.CorrelationId &&
                    cmd.LearnerAccountId == message.LearnerAccountId &&
                    cmd.Category == message.Category &&
                    cmd.Heading == message.Heading &&
                    cmd.Body == message.Body &&
                    cmd.StatusId == 1 &&
                    cmd.NotificationTime == message.NotificationTime &&
                    cmd.TimeToExpire == message.TimeToExpire &&
                    cmd.Link == message.LinkUrl &&
                    cmd.Urgency == (byte)message.Urgency &&
                    cmd.TimeReceived > DateTime.UtcNow.AddSeconds(-5)),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleSendNotification_SetsTimeReceivedToCurrentUtcTime()
        {
            var beforeCall = DateTime.UtcNow;
            var message = new SendNotification
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Heading = "Test",
                Body = "Test",
                Urgency = Urgency.High
            };

            await _sendFunction.Run(message, CancellationToken.None);

            _notificationServiceMock.Verify(s => s.CreateNotificationAsync(
                It.Is<CreateNotificationCommand>(cmd =>
                    cmd.TimeReceived >= beforeCall && cmd.TimeReceived <= DateTime.UtcNow),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleSendNotification_WhenLinkUrlIsNull_PassesNullToCommand()
        {
            var message = new SendNotification
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Heading = "No Link",
                Body = "Body",
                LinkUrl = null,
                Urgency = Urgency.Medium
            };

            await _sendFunction.Run(message, CancellationToken.None);

            _notificationServiceMock.Verify(s => s.CreateNotificationAsync(
                It.Is<CreateNotificationCommand>(cmd => cmd.Link == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleSendNotification_WhenCategoryIsNull_PassesNullToCommand()
        {
            var message = new SendNotification
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Category = null,
                Heading = "No Category",
                Body = "Body",
                Urgency = Urgency.Low
            };

            await _sendFunction.Run(message, CancellationToken.None);

            _notificationServiceMock.Verify(s => s.CreateNotificationAsync(
                It.Is<CreateNotificationCommand>(cmd => cmd.Category == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
