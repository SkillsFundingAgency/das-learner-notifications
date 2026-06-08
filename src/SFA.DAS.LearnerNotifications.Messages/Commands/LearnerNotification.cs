namespace SFA.DAS.LearnerNotifications.Messages.Commands
{
    public class SendNotification
    {
        public Guid CorrelationId { get; set; }
        public Guid LearnerAccountId { get; set; }
        public string Category { get; set; }
        public string Heading { get; set; }
        public string Body { get; set; }
        public string LinkUrl { get; set; }
        public DateTime NotificationTime { get; set; }
        public DateTime TimeToExpire { get; set; }
        public Urgency Urgency { get; set; }
    }
}
