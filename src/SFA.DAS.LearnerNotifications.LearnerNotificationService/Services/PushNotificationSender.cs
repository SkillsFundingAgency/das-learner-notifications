using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.LearnerNotifications.LearnerNotificationService.Infrastructure;
using SFA.DAS.LearnerNotifications.Messages.Commands;

namespace SFA.DAS.LearnerNotifications.LearnerNotificationService.Services;

public interface IPushNotificationSender
{
    Task SendAsync(SendNotification notification, CancellationToken cancellationToken);
}

/// <summary>
/// Sends a SendPushNotificationCommand to the SFA.DAS.PushNotifications NServiceBus endpoint
/// as a native Service Bus message, so this service does not need to host NServiceBus itself.
/// </summary>
public class PushNotificationSender : IPushNotificationSender
{
    public const string EnclosedMessageType =
        "SFA.DAS.PushNotifications.Messages.Commands.SendPushNotificationCommand, SFA.DAS.PushNotifications.Messages";

    private readonly ServiceBusSender _sender;
    private readonly ILogger<PushNotificationSender> _logger;

    public PushNotificationSender(ServiceBusClient serviceBusClient, ILogger<PushNotificationSender> logger)
    {
        _sender = serviceBusClient.CreateSender(QueueNames.PushNotificationsQueue);
        _logger = logger;
    }

    public async Task SendAsync(SendNotification notification, CancellationToken cancellationToken)
    {
        var body = JsonConvert.SerializeObject(new
        {
            ApprenticeAccountIdentifier = notification.LearnerAccountId,
            Title = notification.Heading,
            Body = notification.Body
        });

        var message = new ServiceBusMessage(body)
        {
            MessageId = notification.CorrelationId.ToString(),
            ContentType = "application/json"
        };
        message.ApplicationProperties["NServiceBus.EnclosedMessageTypes"] = EnclosedMessageType;
        message.ApplicationProperties["NServiceBus.MessageIntent"] = "Send";
        message.ApplicationProperties["NServiceBus.ContentType"] = "application/json";
        message.ApplicationProperties["NServiceBus.CorrelationId"] = notification.CorrelationId.ToString();

        await _sender.SendMessageAsync(message, cancellationToken);

        _logger.LogInformation("Push notification sent for CorrelationId {CorrelationId}", notification.CorrelationId);
    }
}
