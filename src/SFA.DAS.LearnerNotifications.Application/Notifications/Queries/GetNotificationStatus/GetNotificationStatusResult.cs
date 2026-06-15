using System;

namespace SFA.DAS.LearnerNotifications.Application.Queries.Results
{
    public class GetNotificationStatusResult
    {
        public byte StatusId { get; set; }
        public string StatusName { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
