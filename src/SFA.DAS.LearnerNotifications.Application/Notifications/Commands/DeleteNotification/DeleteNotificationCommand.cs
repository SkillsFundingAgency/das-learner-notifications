using System;

namespace SFA.DAS.LearnerNotifications.Application.Commands
{
    public class DeleteNotificationCommand
    {
        public Guid AccountIdentifier { get; set; }
        public long NotificationIdentifier { get; set; }
    }
}
