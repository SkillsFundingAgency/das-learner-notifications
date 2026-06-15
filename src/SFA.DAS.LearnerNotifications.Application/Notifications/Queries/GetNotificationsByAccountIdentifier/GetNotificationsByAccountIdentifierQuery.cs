using System;
using System.Collections.Generic;
using SFA.DAS.LearnerNotifications.Application.Models;

namespace SFA.DAS.LearnerNotifications.Application.Queries
{
    public class GetNotificationsByAccountIdentifierQuery
    {
        public Guid AccountIdentifier { get; set; }
        public SortOrder Order { get; set; } = SortOrder.Descending;
        public DateTime? DateFrom { get; set; }
        public List<Status> Statuses { get; set; }
    }
}
