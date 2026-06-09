using System;

namespace SFA.DAS.LearnerNotifications.Domain.Entities
{
    public class StatusHistory
    {
        public long StatusHistoryId { get; set; }
        public long? NotificationId { get; set; }
        public byte? Status { get; set; }
        public DateTime? ChangeDate { get; set; }
    }
}
