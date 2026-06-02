namespace SFA.DAS.LearnerNotifications.Models
{
    public class Notification
    {
        public long Id { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid LearnerAccountId { get; set; }
        public string Category { get; set; }
        public string Heading { get; set; }
        public string Body { get; set; }
        public string LinkUrl { get; set; }
        public NotificationStatus Status { get; set; }
        public DateTime NotificationTime { get; set; }
        public DateTime TimeToExpire { get; set;  }
        public DateTime TimeReceived { get; set; }

    }
}
