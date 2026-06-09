using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LearnerNotifications.Application.Models;
using SFA.DAS.LearnerNotifications.Application.Queries;
using SFA.DAS.LearnerNotifications.Application.Commands;

namespace SFA.DAS.LearnerNotifications.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("learner/")]
    public class LearnerNotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LearnerNotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{accountIdentifier}")]
        public async Task<IActionResult> GetNotifications(
            Guid accountIdentifier,
            [FromQuery] SortOrder order = SortOrder.Descending,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] List<Status> statuses = null)
        {
            var result = await _mediator.Send(new GetNotificationsByAccountIdentifierQuery
            {
                AccountIdentifier = accountIdentifier,
                Order = order,
                DateFrom = dateFrom,
                Statuses = statuses
            });

            if (result == null || result.Notifications.Count == 0)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{accountIdentifier}/notifications/{notificationIdentifier}")]
        public async Task<IActionResult> GetNotification(Guid accountIdentifier, long notificationIdentifier)
        {
            var result = await _mediator.Send(new GetNotificationByIdQuery 
            { 
                AccountIdentifier = accountIdentifier,
                NotificationIdentifier = notificationIdentifier
            });
            
            if (result == null) 
                return NotFound();
                
            return Ok(result);
        }

        [HttpGet("{accountIdentifier}/notifications/{notificationIdentifier}/status")]
        public async Task<IActionResult> GetNotificationStatus(Guid accountIdentifier, long notificationIdentifier)
        {
            var result = await _mediator.Send(new GetNotificationStatusQuery 
            { 
                AccountIdentifier = accountIdentifier,
                NotificationIdentifier = notificationIdentifier
            });
            
            if (result == null) 
                return NotFound();
                
            return Ok(result);
        }

        [HttpPut("{accountIdentifier}/notifications/{notificationIdentifier}/status")]
        public async Task<IActionResult> SetNotificationStatus(Guid accountIdentifier, long notificationIdentifier, [FromBody] SetNotificationStatusRequest request)
        {
            await _mediator.Send(new SetNotificationStatusCommand
            {
                AccountIdentifier = accountIdentifier,
                NotificationIdentifier = notificationIdentifier,
                StatusId = request.StatusId
            });

            return Ok();
        }
        
        [HttpPost("{accountIdentifier}/notifications")]
        public async Task<IActionResult> CreateNotification(Guid accountIdentifier, [FromBody] CreateNotificationRequest request)
        {
            await _mediator.Send(new CreateNotificationCommand
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
            });

            return Ok();
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

        [HttpDelete("{accountIdentifier}/notifications/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(Guid accountIdentifier, long notificationId)
        {
            await _mediator.Send(new DeleteNotificationCommand
            {
                AccountIdentifier = accountIdentifier,
                NotificationIdentifier = notificationId
            });

            return NoContent();
        }

        public class SetNotificationStatusRequest
        {
            public int StatusId { get; set; }
        }
    }
}
