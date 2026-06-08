using SFA.DAS.LearnerNotifications.Application.Data;
using SFA.DAS.LearnerNotifications.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SFA.DAS.LearnerNotifications.Application.Services
{
    public interface INotificationProcessor
    {
        Task Process(SendNotification notification);
    }

    public class NotificationProcessor : INotificationProcessor
    {
        private readonly ILearnerNotificationsDataContext dataContext;

        public NotificationProcessor(ILearnerNotificationsDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task Process(SendNotification notification)
        {
            var model = new Models.Notification
            {
                CorrelationId = notification.CorrelationId,
                LearnerAccountId = notification.LearnerAccountId,
                Category = notification.Category,
                Heading = notification.Heading,
                Body = notification.Body,
                LinkUrl = notification.LinkUrl,
                NotificationTime = notification.NotificationTime,
                TimeToExpire = notification.TimeToExpire,
                Urgency = Convert(notification.Urgency)
            };

            await dataContext.SaveNotification(model);
        }

        private Models.NotificationUrgency Convert(Urgency urgency)
        {
            return urgency switch
            {
                Urgency.Low => Models.NotificationUrgency.Low,
                Urgency.Medium => Models.NotificationUrgency.Medium,
                Urgency.High => Models.NotificationUrgency.High,
                _ => throw new ArgumentOutOfRangeException(nameof(urgency), $"Unexpected urgency value: {urgency}")
            };
        }
    }
}
