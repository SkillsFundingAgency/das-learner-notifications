using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LearnerNotifications.Application.Models;
using SFA.DAS.LearnerNotifications.Application.Queries;
using SFA.DAS.LearnerNotifications.Application.Commands;
using SFA.DAS.LearnerNotifications.Application.Notifications;

namespace SFA.DAS.LearnerNotifications.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("learner/")]
    public class LearnerNotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public LearnerNotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("{accountIdentifier}")]
        public async Task<IActionResult> GetNotifications(
            Guid accountIdentifier,
            [FromQuery] SortOrder order = SortOrder.Descending,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] List<Status> statuses = null)
        {
            var query = new GetNotificationsByAccountIdentifierQuery
            {
                AccountIdentifier = accountIdentifier,
                Order = order,
                DateFrom = dateFrom,
                Statuses = statuses
            };
            var result = await _notificationService.GetNotificationsByAccountAsync(query, HttpContext.RequestAborted);

            if (result == null || result.Notifications.Count == 0)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{accountIdentifier}/notifications/{notificationIdentifier}")]
        public async Task<IActionResult> GetNotification(Guid accountIdentifier, long notificationIdentifier)
        {
            var query = new GetNotificationByIdQuery
            {
                AccountIdentifier = accountIdentifier,
                NotificationIdentifier = notificationIdentifier
            };
            var result = await _notificationService.GetNotificationByIdAsync(query, HttpContext.RequestAborted);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{accountIdentifier}/notifications/{notificationIdentifier}/status")]
        public async Task<IActionResult> GetNotificationStatus(Guid accountIdentifier, long notificationIdentifier)
        {
            var query = new GetNotificationStatusQuery
            {
                AccountIdentifier = accountIdentifier,
                NotificationIdentifier = notificationIdentifier
            };
            var result = await _notificationService.GetNotificationStatusAsync(query, HttpContext.RequestAborted);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{accountIdentifier}/notifications/{notificationIdentifier}/status")]
        public async Task<IActionResult> SetNotificationStatus(Guid accountIdentifier, long notificationIdentifier, [FromBody] SetNotificationStatusRequest request)
        {
            var command = new SetNotificationStatusCommand
            {
                AccountIdentifier = accountIdentifier,
                NotificationIdentifier = notificationIdentifier,
                StatusId = request.StatusId
            };
            await _notificationService.SetNotificationStatusAsync(command, HttpContext.RequestAborted);
            return Ok();
        }

        [HttpPost("{accountIdentifier}/notifications")]
        public async Task<IActionResult> CreateNotification(Guid accountIdentifier, [FromBody] CreateNotificationRequest request)
        {
            var command = new CreateNotificationCommand
            {
                CorrelationId = request.CorrelationId,
                LearnerAccountId = accountIdentifier,
                Category = request.Category,
                Heading = request.Heading,
                Body = request.Body,
                StatusId = request.StatusId,
                NotificationTime = request.NotificationTime,
                TimeToExpire = request.TimeToExpire,
                TimeReceived = request.TimeReceived,
                Link = request.Link,
                Urgency = request.Urgency
            };
            await _notificationService.CreateNotificationAsync(command, HttpContext.RequestAborted);
            return Ok();
        }

        [HttpDelete("{accountIdentifier}/notifications/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(Guid accountIdentifier, long notificationId)
        {
            var command = new DeleteNotificationCommand
            {
                AccountIdentifier = accountIdentifier,
                NotificationIdentifier = notificationId
            };
            await _notificationService.DeleteNotificationAsync(command, HttpContext.RequestAborted);
            return NoContent();
        }

        public class CreateNotificationRequest
        {
            public Guid CorrelationId { get; set; }
            public string Category { get; set; } = null!;
            public string Heading { get; set; } = null!;
            public string Body { get; set; } = null!;
            public byte StatusId { get; set; }
            public DateTime NotificationTime { get; set; }
            public DateTime TimeToExpire { get; set; }
            public DateTime TimeReceived { get; set; }
            public string? Link { get; set; }
            public byte Urgency { get; set; }
        }

        public class SetNotificationStatusRequest
        {
            public int StatusId { get; set; }
        }
    }
}
