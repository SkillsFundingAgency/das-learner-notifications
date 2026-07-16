using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.LearnerNotifications.Application.Notifications;

namespace SFA.DAS.LearnerNotifications.LearnerNotificationService.Functions;

public class CleanupExpiredNotifications
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<CleanupExpiredNotifications> _logger;

    public CleanupExpiredNotifications(INotificationService notificationService, ILogger<CleanupExpiredNotifications> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [Function("CleanupExpiredNotifications")]
    public async Task Run(
        [TimerTrigger("0 0 2 * * *")] TimerInfo timer,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Running expired notifications cleanup");
            var deleted = await _notificationService.DeleteExpiredNotificationsAsync(cancellationToken);
            _logger.LogInformation("Deleted {Count} expired notifications", deleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Expired notifications cleanup job has failed");
            throw;
        }
    }
}
