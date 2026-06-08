using System;
using MediatR;
using SFA.DAS.LearnerNotifications.Domain.Entities;

namespace SFA.DAS.LearnerNotifications.Application.Queries
{
    public class GetNotificationByIdQuery : IRequest<Notification>
    {
        public Guid AccountIdentifier { get; set; }
        public long NotificationIdentifier { get; set; }
    }
}
