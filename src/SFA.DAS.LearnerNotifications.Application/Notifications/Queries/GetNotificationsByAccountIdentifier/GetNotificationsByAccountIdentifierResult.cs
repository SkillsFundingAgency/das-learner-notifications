using System.Collections.Generic;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Application.Queries.Results
{
    public class GetNotificationsByAccountIdentifierResult
    {
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
