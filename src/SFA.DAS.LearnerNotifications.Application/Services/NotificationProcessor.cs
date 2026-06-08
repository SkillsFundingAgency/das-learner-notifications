using SFA.DAS.LearnerNotifications.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LearnerNotifications.Application.Services
{
    public interface INotificationProcessor
    {
        Task Process(SendNotification notification);
    }

    public class NotificationProcessor : INotificationProcessor
    {
        public Task Process(SendNotification notification)
        {
            throw new NotImplementedException();
        }
    }
}
