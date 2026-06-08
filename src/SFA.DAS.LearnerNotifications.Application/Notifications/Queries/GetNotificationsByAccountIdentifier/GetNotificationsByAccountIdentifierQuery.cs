using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.LearnerNotifications.Application.Models;
using SFA.DAS.LearnerNotifications.Application.Queries.Results;

namespace SFA.DAS.LearnerNotifications.Application.Queries
{
    public class GetNotificationsByAccountIdentifierQuery : IRequest<GetNotificationsByAccountIdentifierResult>
    {
        public Guid AccountIdentifier { get; set; }
        public SortOrder Order { get; set; } = SortOrder.Descending;
        public DateTime? DateFrom { get; set; }
        public List<Status> Statuses { get; set; }
    }
}
