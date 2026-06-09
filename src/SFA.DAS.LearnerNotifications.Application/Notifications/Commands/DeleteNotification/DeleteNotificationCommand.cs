using System;
using MediatR;

namespace SFA.DAS.LearnerNotifications.Application.Commands
{
    public class DeleteNotificationCommand : IRequest
    {
        public Guid AccountIdentifier { get; set; }
        public long NotificationIdentifier { get; set; }
    }
}
