using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LearnerNotifications.LearnerNotificationService.Handlers
{
    public class SendLearnerNotificationsHandler : IHandleMessages<SFA.DAS.LearnerNotifications.Messages.Commands.SendNotification>
    {
        private readonly ILogger<SendLearnerNotificationsHandler> logger;

        public SendLearnerNotificationsHandler(ILogger<SendLearnerNotificationsHandler> logger)
        {
            this.logger = logger;
        }

        public async Task Handle(LearnerNotifications.Messages.Commands.SendNotification message, IMessageHandlerContext context)
        {
            logger.LogInformation($"Processing learner notification. CorrelationId: {message.CorrelationId}, Heading: {message.Heading}");            
        }
    }
}
