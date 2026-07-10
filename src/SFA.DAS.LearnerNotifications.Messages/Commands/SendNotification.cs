using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.LearnerNotifications.Messages.Commands
{
    [ExcludeFromCodeCoverage]
    public class SendNotification
    {
        public Guid CorrelationId { get; set; }
        public Guid LearnerAccountId { get; set; }
        public string Category { get; set; }
        public string Heading { get; set; }
        public string Body { get; set; }
        public string LinkUrl { get; set; }
        public DateTime NotificationTime { get; set; }
        public DateTime TimeToExpire { get; set; } = DateTime.UtcNow.AddMonths(3);
        public Urgency Urgency { get; set; } = Urgency.Low;
    }

    public enum Urgency
    {
        Low = 1,
        Medium = 2,
        High = 3
    }
}
