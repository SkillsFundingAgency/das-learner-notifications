using System;

namespace SFA.DAS.LearnerNotifications.Application.Queries
{
    public class GetNotificationStatusQuery
    {
        public Guid AccountIdentifier { get; set; }
        public long NotificationIdentifier { get; set; }
    }
}
