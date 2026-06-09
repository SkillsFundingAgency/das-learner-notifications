using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ILearnerNotificationsDataContext> logger;

        public NotificationProcessor(ILearnerNotificationsDataContext dataContext, ILogger<ILearnerNotificationsDataContext> logger)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.logger = logger;
        }

        public async Task Process(SendNotification notification)
        {
            logger.LogDebug($"Processing the notification with CorrelationId: {notification.CorrelationId}, heading: {notification.Heading}");
            try
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
                    Urgency = Convert(notification.Urgency),
                    Status = Models.NotificationStatus.Unread,
                };

                await dataContext.SaveNotification(model);
                logger.LogInformation($"Finished processing the notification with CorrelationId: {notification.CorrelationId}, heading: {notification.Heading}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while processing notification. Error: {ex.Message}");
                throw;
            }
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
