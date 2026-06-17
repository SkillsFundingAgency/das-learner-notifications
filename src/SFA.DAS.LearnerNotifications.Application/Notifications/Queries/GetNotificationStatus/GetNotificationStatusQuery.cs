using System;
using MediatR;
using SFA.DAS.LearnerNotifications.Application.Queries.Results;

namespace SFA.DAS.LearnerNotifications.Application.Queries
{
    public class GetNotificationStatusQuery : IRequest<GetNotificationStatusResult>
    {
        public Guid AccountIdentifier { get; set; }
        public long NotificationIdentifier { get; set; }
    }
}
