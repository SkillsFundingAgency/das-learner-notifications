using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.LearnerNotifications.LearnerNotificationService.Configuration;

[ExcludeFromCodeCoverage]
public class ApplicationConfiguration
{
    public string SqlConnectionString { get; set; }
}