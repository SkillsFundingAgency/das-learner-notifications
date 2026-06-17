using System;
using MediatR;

namespace SFA.DAS.LearnerNotifications.Application.Commands
{
    public class SetNotificationStatusCommand : IRequest
    {
        public Guid AccountIdentifier { get; set; }
        public long NotificationIdentifier { get; set; }
        public int StatusId { get; set; }
    }
}
