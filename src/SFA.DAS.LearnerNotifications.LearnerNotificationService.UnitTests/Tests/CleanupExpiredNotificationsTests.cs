using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerNotifications.Application.Notifications;
using SFA.DAS.LearnerNotifications.LearnerNotificationService.Functions;

namespace SFA.DAS.LearnerNotifications.LearnerNotificationService.UnitTests.Tests
{
    [TestFixture]
    public class CleanupExpiredNotificationsTests
    {
        private Mock<INotificationService> _notificationServiceMock;
        private Mock<ILogger<CleanupExpiredNotifications>> _cleanupLoggerMock;
        private CleanupExpiredNotifications _cleanupFunction;

        [SetUp]
        public void SetUp()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _cleanupLoggerMock = new Mock<ILogger<CleanupExpiredNotifications>>();
            _cleanupFunction = new CleanupExpiredNotifications(_notificationServiceMock.Object, _cleanupLoggerMock.Object);
        }

        [Test]
        public async Task CleanupExpiredNotifications_CallsDeleteExpiredNotificationsAsync()
        {
            _notificationServiceMock
                .Setup(s => s.DeleteExpiredNotificationsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            await _cleanupFunction.Run(It.IsAny<TimerInfo>(), CancellationToken.None);

            _notificationServiceMock.Verify(s => s.DeleteExpiredNotificationsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task CleanupExpiredNotifications_LogsInformationWhenStartingAndCompleting()
        {
            _notificationServiceMock
                .Setup(s => s.DeleteExpiredNotificationsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(3);

            await _cleanupFunction.Run(It.IsAny<TimerInfo>(), CancellationToken.None);

            _cleanupLoggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Running expired notifications cleanup")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _cleanupLoggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Deleted 3 expired notifications")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public void CleanupExpiredNotifications_WhenServiceThrowsException_LogsErrorAndThrows()
        {
            var expectedException = new Exception("Cleanup failed");
            _notificationServiceMock
                .Setup(s => s.DeleteExpiredNotificationsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            var ex = Assert.ThrowsAsync<Exception>(() =>
                _cleanupFunction.Run(It.IsAny<TimerInfo>(), CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Cleanup failed"));

            _cleanupLoggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Expired notifications cleanup job has failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task CleanupExpiredNotifications_WhenNoExpiredNotifications_LogsZeroCount()
        {
            _notificationServiceMock
                .Setup(s => s.DeleteExpiredNotificationsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            await _cleanupFunction.Run(It.IsAny<TimerInfo>(), CancellationToken.None);

            _cleanupLoggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Deleted 0 expired notifications")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
