using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.LearnerNotifications.Application.Commands;
using SFA.DAS.LearnerNotifications.Application.Notifications;
using SFA.DAS.LearnerNotifications.Messages.Commands;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.LearnerNotifications.LearnerNotificationService.Functions;

[ExcludeFromCodeCoverage]
public class HandleSendNotification
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<HandleSendNotification> _logger;

    public HandleSendNotification(INotificationService notificationService, ILogger<HandleSendNotification> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [Function(nameof(HandleSendNotification))]
    public async Task Run(
        [ServiceBusTrigger("%EndpointName%", Connection = "ServiceBusConnectionString")]
        SendNotification message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received notification: {CorrelationId}", message.CorrelationId);

        if (string.IsNullOrWhiteSpace(message.Heading))
        {
            var ex = new ArgumentException("Heading is required");
            _logger.LogError(ex, "Invalid notification: Heading missing for CorrelationId {CorrelationId}", message.CorrelationId);
            throw ex;
        }

        var command = new CreateNotificationCommand
        {
            CorrelationId = message.CorrelationId,
            LearnerAccountId = message.LearnerAccountId,
            Category = message.Category,
            Heading = message.Heading,
            Body = message.Body,
            StatusId = 1,
            NotificationTime = message.NotificationTime,
            TimeToExpire = message.TimeToExpire,
            TimeReceived = DateTime.UtcNow,
            Link = message.LinkUrl,
            Urgency = (byte)message.Urgency
        };

        try
        {
            await _notificationService.CreateNotificationAsync(command, cancellationToken);
            _logger.LogInformation("Notification saved: {CorrelationId}", message.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing notification: {CorrelationId}", message.CorrelationId);
            throw;
        }
    }
}
