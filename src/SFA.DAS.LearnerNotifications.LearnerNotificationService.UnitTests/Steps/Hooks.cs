using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.LearnerNotifications.LearnerNotificationService.UnitTests.Steps;

[Binding]
public class Hooks
{
    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        return new ServiceCollection();
    }
}
