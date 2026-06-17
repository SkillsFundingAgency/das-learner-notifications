using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.LearnerNotifications.Application.Queries.Results;
using SFA.DAS.LearnerNotifications.Domain.Entities;
using SFA.DAS.LearnerNotifications.Models;

namespace SFA.DAS.LearnerNotifications.Application.Queries
{
    public class GetNotificationsByAccountIdentifierQuery : IRequest<GetNotificationsByAccountIdentifierResult>
    {
        public Guid AccountIdentifier { get; set; }
        public SortOrder Order { get; set; } = SortOrder.Descending;
        public DateTime? DateFrom { get; set; }
        public List<NotificationStatus> Statuses { get; set; }
    }
}
