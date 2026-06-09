using System;
using MediatR;

namespace SFA.DAS.LearnerNotifications.Application.Commands
{
    public class CreateNotificationCommand : IRequest
    {
        public Guid CorrelationId { get; set; }
        public Guid LearnerAccountId { get; set; }
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
}
