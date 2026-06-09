using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.LearnerNotifications.Domain.Configuration
{
    [ExcludeFromCodeCoverage]
    public class LearnerNotificationsConfiguration
    {
        public string SqlConnectionString { get ; set ; }
    }
}