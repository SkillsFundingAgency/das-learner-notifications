using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NServiceBus.Transport.AzureServiceBus.AdvancedExtensibility;

namespace SFA.DAS.LearnerNotifications.LearnerNotificationService;

public class HandleSendNotificaiton
{
    private readonly ILogger<HandleSendNotificaiton> logger;

    public HandleSendNotificaiton(ILogger<HandleSendNotificaiton> logger)
    {
        this.logger = logger;
    }

    [Function(nameof(HandleSendNotificaiton))]
    public async Task Run(
        [ServiceBusTrigger("%LearnerNotificationsEndpoint%", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    { 
        logger.LogInformation($"Received message with ID: {message.MessageId}, Content: {message.Body}");
        //deserialise message to Commands.SendLearnerNotification
        //User application.notificationprocessor to process the message
    }
}
