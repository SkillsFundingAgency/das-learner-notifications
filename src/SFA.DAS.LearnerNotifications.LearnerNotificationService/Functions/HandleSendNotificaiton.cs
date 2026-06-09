using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NServiceBus.Transport.AzureServiceBus.AdvancedExtensibility;
using SFA.DAS.LearnerNotifications.Application.Services;

namespace SFA.DAS.LearnerNotifications.LearnerNotificationService.Functions;

public class HandleSendNotificaiton
{
    private readonly ILogger<HandleSendNotificaiton> logger;
    private readonly INotificationProcessor processor;

    public HandleSendNotificaiton(ILogger<HandleSendNotificaiton> logger, INotificationProcessor processor)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
    }

    [Function(nameof(HandleSendNotificaiton))]
    public async Task Run(
        [ServiceBusTrigger("%LearnerNotificationsEndpoint%", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    { 
        logger.LogInformation($"Received message with ID: {message.MessageId}, Content: {message.Body}");
        //deserialise message to Commands.SendLearnerNotification
        var notification = message.Body.ToObjectFromJson<Messages.Commands.SendNotification>();
        logger.LogInformation($"Notificaiotn: Heading: {notification.Heading}, Body: {notification.Body}, LinkUrl: {notification.LinkUrl}, Category: {notification.Category}, LearnerAccountId: {notification.LearnerAccountId}, CorrelationId: {notification.CorrelationId}, NotificationTime: {notification.NotificationTime}, TimeToExpire: {notification.TimeToExpire}, Urgency: {notification.Urgency}");
        //User application.notificationprocessor to process the message
        await processor.Process(notification);
    }
}
