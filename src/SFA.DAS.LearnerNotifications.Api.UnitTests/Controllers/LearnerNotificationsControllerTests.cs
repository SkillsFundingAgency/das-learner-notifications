using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerNotifications.Api.Controllers;
using SFA.DAS.LearnerNotifications.Application.Commands;
using SFA.DAS.LearnerNotifications.Application.Models;
using SFA.DAS.LearnerNotifications.Application.Queries;
using SFA.DAS.LearnerNotifications.Application.Queries.Results;
using SFA.DAS.LearnerNotifications.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using Status = SFA.DAS.LearnerNotifications.Application.Models.Status;

namespace SFA.DAS.LearnerNotifications.UnitTests
{
    [TestFixture]
    public class LearnerNotificationsControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private LearnerNotificationsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new LearnerNotificationsController(_mediatorMock.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        // ==================== GetNotifications Tests ====================

        [Test, MoqAutoData]
        public async Task GetNotifications_ReturnsNotFound_WhenNoNotificationsExist(Guid accountIdentifier)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationsByAccountIdentifierQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetNotificationsByAccountIdentifierResult { Notifications = new List<Notification>() });

            // Act
            var result = await _controller.GetNotifications(accountIdentifier);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test, MoqAutoData]
        public async Task GetNotifications_ReturnsOk_WithNotifications(Guid accountIdentifier, List<Notification> notifications)
        {
            // Arrange
            var expectedResult = new GetNotificationsByAccountIdentifierResult { Notifications = notifications };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationsByAccountIdentifierQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetNotifications(accountIdentifier);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test, MoqAutoData]
        public async Task GetNotifications_PassesOrderParameter_ToMediator(Guid accountIdentifier)
        {
            // Arrange
            var order = SortOrder.Ascending;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationsByAccountIdentifierQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetNotificationsByAccountIdentifierResult { Notifications = new List<Notification>() });

            // Act
            await _controller.GetNotifications(accountIdentifier, order: order);

            // Assert
            _mediatorMock.Verify(m => m.Send(
                It.Is<GetNotificationsByAccountIdentifierQuery>(q => q.Order == order),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task GetNotifications_PassesDateFromParameter_ToMediator(Guid accountIdentifier, DateTime dateFrom)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationsByAccountIdentifierQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetNotificationsByAccountIdentifierResult { Notifications = new List<Notification>() });

            // Act
            await _controller.GetNotifications(accountIdentifier, dateFrom: dateFrom);

            // Assert
            _mediatorMock.Verify(m => m.Send(
                It.Is<GetNotificationsByAccountIdentifierQuery>(q => q.DateFrom == dateFrom),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task GetNotifications_PassesStatusesParameter_ToMediator(Guid accountIdentifier, List<Status> statuses)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationsByAccountIdentifierQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetNotificationsByAccountIdentifierResult { Notifications = new List<Notification>() });

            // Act
            await _controller.GetNotifications(accountIdentifier, statuses: statuses);

            // Assert
            _mediatorMock.Verify(m => m.Send(
                It.Is<GetNotificationsByAccountIdentifierQuery>(q => q.Statuses == statuses),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task GetNotifications_PassesAllParameters_ToMediator(Guid accountIdentifier, DateTime dateFrom, List<Status> statuses)
        {
            // Arrange
            var order = SortOrder.Descending;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationsByAccountIdentifierQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetNotificationsByAccountIdentifierResult { Notifications = new List<Notification>() });

            // Act
            await _controller.GetNotifications(accountIdentifier, order, dateFrom, statuses);

            // Assert
            _mediatorMock.Verify(m => m.Send(
                It.Is<GetNotificationsByAccountIdentifierQuery>(q => 
                    q.Order == order && 
                    q.DateFrom == dateFrom && 
                    q.Statuses == statuses),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task GetNotifications_UsesDefaultOrder_WhenNotProvided(Guid accountIdentifier)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationsByAccountIdentifierQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetNotificationsByAccountIdentifierResult { Notifications = new List<Notification>() });

            // Act
            await _controller.GetNotifications(accountIdentifier);

            // Assert
            _mediatorMock.Verify(m => m.Send(
                It.Is<GetNotificationsByAccountIdentifierQuery>(q => q.Order == SortOrder.Descending),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ==================== GetNotification Tests ====================

        [Test, MoqAutoData]
        public async Task GetNotification_ReturnsNotFound_WhenNotificationDoesNotExist(Guid accountIdentifier, long notificationIdentifier)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Notification)null);

            // Act
            var result = await _controller.GetNotification(accountIdentifier, notificationIdentifier);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test, MoqAutoData]
        public async Task GetNotification_ReturnsOk_WithNotification(Guid accountIdentifier, long notificationIdentifier, Notification notification)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(notification);

            // Act
            var result = await _controller.GetNotification(accountIdentifier, notificationIdentifier);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(notification);
        }

        [Test, MoqAutoData]
        public async Task GetNotification_PassesCorrectIds_ToMediator(Guid accountIdentifier, long notificationIdentifier)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Notification());

            // Act
            await _controller.GetNotification(accountIdentifier, notificationIdentifier);

            // Assert
            _mediatorMock.Verify(m => m.Send(
                It.Is<GetNotificationByIdQuery>(q => q.AccountIdentifier == accountIdentifier && q.NotificationIdentifier == notificationIdentifier),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ==================== GetNotificationStatus Tests ====================

        [Test, MoqAutoData]
        public async Task GetNotificationStatus_ReturnsNotFound_WhenStatusDoesNotExist(Guid accountIdentifier, long notificationIdentifier)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationStatusQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetNotificationStatusResult)null);

            // Act
            var result = await _controller.GetNotificationStatus(accountIdentifier, notificationIdentifier);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test, MoqAutoData]
        public async Task GetNotificationStatus_ReturnsOk_WithStatus(Guid accountIdentifier, long notificationIdentifier, GetNotificationStatusResult statusResult)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNotificationStatusQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(statusResult);

            // Act
            var result = await _controller.GetNotificationStatus(accountIdentifier, notificationIdentifier);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(statusResult);
        }

        // ==================== SetNotificationStatus Tests ====================

        [Test, MoqAutoData]
        public async Task SetNotificationStatus_ReturnsOk_Always(Guid accountIdentifier, long notificationIdentifier, LearnerNotificationsController.SetNotificationStatusRequest request)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<SetNotificationStatusCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SetNotificationStatus(accountIdentifier, notificationIdentifier, request);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Test, MoqAutoData]
        public async Task SetNotificationStatus_PassesCorrectCommand_ToMediator(Guid accountIdentifier, long notificationIdentifier, LearnerNotificationsController.SetNotificationStatusRequest request)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<SetNotificationStatusCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.SetNotificationStatus(accountIdentifier, notificationIdentifier, request);

            // Assert
            _mediatorMock.Verify(m => m.Send(
                It.Is<SetNotificationStatusCommand>(cmd => 
                    cmd.AccountIdentifier == accountIdentifier && 
                    cmd.NotificationIdentifier == notificationIdentifier && 
                    cmd.StatusId == request.StatusId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ==================== CreateNotification Tests ====================

        [Test, MoqAutoData]
        public async Task CreateNotification_ReturnsOk_WhenSuccessful(Guid accountIdentifier, LearnerNotificationsController.CreateNotificationRequest request)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateNotificationCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateNotification(accountIdentifier, request);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Test, MoqAutoData]
        public async Task CreateNotification_PassesCorrectCommand_ToMediator(Guid accountIdentifier, LearnerNotificationsController.CreateNotificationRequest request)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateNotificationCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.CreateNotification(accountIdentifier, request);

            // Assert
            _mediatorMock.Verify(m => m.Send(
                It.Is<CreateNotificationCommand>(cmd => 
                    cmd.CorrelationId == request.CorrelationId &&
                    cmd.LearnerAccountId == accountIdentifier &&
                    cmd.Category == request.Category &&
                    cmd.Heading == request.Heading &&
                    cmd.Body == request.Body &&
                    cmd.StatusId == request.StatusId &&
                    cmd.NotificationTime == request.NotificationTime &&
                    cmd.TimeToExpire == request.TimeToExpire &&
                    cmd.TimeReceived == request.TimeReceived &&
                    cmd.Link == request.Link &&
                    cmd.Urgency == request.Urgency),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ==================== DeleteNotification Tests ====================

        [Test, MoqAutoData]
        public async Task DeleteNotification_ReturnsNoContent_Always(Guid accountIdentifier, long notificationId)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteNotificationCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteNotification(accountIdentifier, notificationId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Test, MoqAutoData]
        public async Task DeleteNotification_PassesCorrectCommand_ToMediator(Guid accountIdentifier, long notificationId)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteNotificationCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.DeleteNotification(accountIdentifier, notificationId);

            // Assert
            _mediatorMock.Verify(m => m.Send(
                It.Is<DeleteNotificationCommand>(cmd => 
                    cmd.AccountIdentifier == accountIdentifier && 
                    cmd.NotificationIdentifier == notificationId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // ==================== Model Tests (unchanged but kept) ====================

        [Test]
        public void CreateNotificationRequest_Model_Test()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var notificationTime = DateTime.UtcNow;
            var timeToExpire = DateTime.UtcNow.AddDays(7);
            var timeReceived = DateTime.UtcNow.AddMinutes(-5);

            var sut = new LearnerNotificationsController.CreateNotificationRequest
            {
                CorrelationId = correlationId,
                Category = "TrainingReminder",
                Heading = "Important Training Due",
                Body = "Please complete your mandatory training by Friday.",
                StatusId = 0,
                NotificationTime = notificationTime,
                TimeToExpire = timeToExpire,
                TimeReceived = timeReceived,
                Link = "/training/123",
                Urgency = 2
            };

            // Assert
            sut.CorrelationId.Should().Be(correlationId);
            sut.Category.Should().Be("TrainingReminder");
            sut.Heading.Should().Be("Important Training Due");
            sut.Body.Should().Be("Please complete your mandatory training by Friday.");
            sut.StatusId.Should().Be(0);
            sut.NotificationTime.Should().Be(notificationTime);
            sut.TimeToExpire.Should().Be(timeToExpire);
            sut.TimeReceived.Should().Be(timeReceived);
            sut.Link.Should().Be("/training/123");
            sut.Urgency.Should().Be(2);
        }

        [Test]
        public void CreateNotificationRequest_Model_WithNullLink_Test()
        {
            // Arrange
            var sut = new LearnerNotificationsController.CreateNotificationRequest
            {
                CorrelationId = Guid.NewGuid(),
                Category = "SystemAlert",
                Heading = "System Maintenance",
                Body = "System will be down for maintenance.",
                StatusId = 1,
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(1),
                TimeReceived = DateTime.UtcNow,
                Link = null,
                Urgency = 0
            };

            // Assert
            sut.CorrelationId.Should().NotBe(Guid.Empty);
            sut.Category.Should().Be("SystemAlert");
            sut.Heading.Should().Be("System Maintenance");
            sut.Body.Should().Be("System will be down for maintenance.");
            sut.StatusId.Should().Be(1);
            sut.Link.Should().BeNull();
            sut.Urgency.Should().Be(0);
        }

        [Test]
        public void SetNotificationStatusRequest_Model_Test()
        {
            // Arrange
            var sut = new LearnerNotificationsController.SetNotificationStatusRequest
            {
                StatusId = 3
            };

            // Assert
            sut.StatusId.Should().Be(3);
        }

        [Test]
        public void CreateNotificationRequest_DefaultValues_Test()
        {
            // Arrange
            var sut = new LearnerNotificationsController.CreateNotificationRequest();

            // Assert
            sut.CorrelationId.Should().Be(Guid.Empty);
            sut.Category.Should().BeNull();
            sut.Heading.Should().BeNull();
            sut.Body.Should().BeNull();
            sut.StatusId.Should().Be(0);
            sut.NotificationTime.Should().Be(DateTime.MinValue);
            sut.TimeToExpire.Should().Be(DateTime.MinValue);
            sut.TimeReceived.Should().Be(DateTime.MinValue);
            sut.Link.Should().BeNull();
            sut.Urgency.Should().Be(0);
        }

        [Test]
        public void CreateNotificationRequest_AllProperties_CanBeSet()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            // Act
            var sut = new LearnerNotificationsController.CreateNotificationRequest
            {
                CorrelationId = correlationId,
                Category = "Test",
                Heading = "Test Heading",
                Body = "Test Body",
                StatusId = 1,
                NotificationTime = now,
                TimeToExpire = now.AddDays(1),
                TimeReceived = now,
                Link = "test-link",
                Urgency = 1
            };

            // Assert
            sut.CorrelationId.Should().Be(correlationId);
            sut.Category.Should().Be("Test");
            sut.Heading.Should().Be("Test Heading");
            sut.Body.Should().Be("Test Body");
            sut.StatusId.Should().Be(1);
            sut.NotificationTime.Should().Be(now);
            sut.TimeToExpire.Should().Be(now.AddDays(1));
            sut.TimeReceived.Should().Be(now);
            sut.Link.Should().Be("test-link");
            sut.Urgency.Should().Be(1);
        }

        [Test, MoqAutoData]
        public async Task CreateNotification_HandlesDifferentUrgencyLevels(Guid accountIdentifier)
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateNotificationCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var requestLow = new LearnerNotificationsController.CreateNotificationRequest
            {
                CorrelationId = Guid.NewGuid(),
                Category = "LowPriority",
                Heading = "Low Urgency",
                Body = "This is low urgency",
                StatusId = 0,
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(30),
                TimeReceived = DateTime.UtcNow,
                Link = null,
                Urgency = 0
            };

            var requestHigh = new LearnerNotificationsController.CreateNotificationRequest
            {
                CorrelationId = Guid.NewGuid(),
                Category = "HighPriority",
                Heading = "High Urgency",
                Body = "This is high urgency",
                StatusId = 0,
                NotificationTime = DateTime.UtcNow,
                TimeToExpire = DateTime.UtcNow.AddDays(1),
                TimeReceived = DateTime.UtcNow,
                Link = "/urgent",
                Urgency = 2
            };

            // Act & Assert
            var resultLow = await _controller.CreateNotification(accountIdentifier, requestLow);
            resultLow.Should().BeOfType<OkResult>();

            var resultHigh = await _controller.CreateNotification(accountIdentifier, requestHigh);
            resultHigh.Should().BeOfType<OkResult>();
        }
    }
}
