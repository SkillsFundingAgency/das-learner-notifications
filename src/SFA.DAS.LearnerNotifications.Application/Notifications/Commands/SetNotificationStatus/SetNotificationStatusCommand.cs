using System;

namespace SFA.DAS.LearnerNotifications.Application.Commands
{
    public class SetNotificationStatusCommand
    {
        public Guid AccountIdentifier { get; set; }
        public long NotificationIdentifier { get; set; }
        public int StatusId { get; set; }
    }
}
