using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SFA.DAS.LearnerNotifications.LearnerNotificationService.Infrastructure;
using SFA.DAS.LearnerNotifications.LearnerNotificationService.Services;
using SFA.DAS.LearnerNotifications.Messages.Commands;

namespace SFA.DAS.LearnerNotifications.LearnerNotificationService.UnitTests.Tests
{
    [TestFixture]
    public class PushNotificationSenderTests
    {
        private Mock<ServiceBusClient> _clientMock;
        private Mock<ServiceBusSender> _senderMock;
        private ServiceBusMessage? _sentMessage;
        private PushNotificationSender _pushNotificationSender;

        [SetUp]
        public void SetUp()
        {
            _sentMessage = null;
            _senderMock = new Mock<ServiceBusSender>();
            _senderMock
                .Setup(s => s.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .Callback<ServiceBusMessage, CancellationToken>((m, _) => _sentMessage = m)
                .Returns(Task.CompletedTask);

            _clientMock = new Mock<ServiceBusClient>();
            _clientMock
                .Setup(c => c.CreateSender(QueueNames.PushNotificationsQueue))
                .Returns(_senderMock.Object);

            _pushNotificationSender = new PushNotificationSender(_clientMock.Object, Mock.Of<ILogger<PushNotificationSender>>());
        }

        [Test]
        public void Constructor_CreatesSenderForPushNotificationsQueue()
        {
            _clientMock.Verify(c => c.CreateSender("SFA.DAS.PushNotifications"), Times.Once);
        }

        [Test]
        public async Task SendAsync_MapsNotificationToSendPushNotificationCommandBody()
        {
            var notification = new SendNotification
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Heading = "Heading",
                Body = "Body",
                LinkUrl = "/slug"
            };

            await _pushNotificationSender.SendAsync(notification, CancellationToken.None);

            Assert.That(_sentMessage, Is.Not.Null);
            var body = JObject.Parse(_sentMessage!.Body.ToString());
            Assert.That(body["ApprenticeAccountIdentifier"]!.Value<string>(), Is.EqualTo(notification.LearnerAccountId.ToString()));
            Assert.That(body["Title"]!.Value<string>(), Is.EqualTo("Heading"));
            Assert.That(body["Body"]!.Value<string>(), Is.EqualTo("Body"));
        }

        [Test]
        public async Task SendAsync_SetsNServiceBusHeaders()
        {
            var notification = new SendNotification
            {
                CorrelationId = Guid.NewGuid(),
                LearnerAccountId = Guid.NewGuid(),
                Heading = "Heading",
                Body = "Body"
            };

            await _pushNotificationSender.SendAsync(notification, CancellationToken.None);

            Assert.That(_sentMessage!.MessageId, Is.EqualTo(notification.CorrelationId.ToString()));
            Assert.That(_sentMessage.ContentType, Is.EqualTo("application/json"));
            Assert.That(_sentMessage.ApplicationProperties["NServiceBus.EnclosedMessageTypes"],
                Is.EqualTo("SFA.DAS.PushNotifications.Messages.Commands.SendPushNotificationCommand, SFA.DAS.PushNotifications.Messages"));
            Assert.That(_sentMessage.ApplicationProperties["NServiceBus.MessageIntent"], Is.EqualTo("Send"));
        }
    }
}
