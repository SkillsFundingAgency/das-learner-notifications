using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;
using SFA.DAS.LearnerNotifications.Tests.Acceptance.Data;

namespace SFA.DAS.LearnerNotifications.Tests.Acceptance.StepDefinitions
{
    public class SetUpDependencies 
    {
        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();            
            services.AddScoped<TestingContext>();
            services.AddScoped<MessageSession>();
            services.AddScoped<TestSessionDataContext>(options =>
                new TestSessionDataContext(TestRunBindings.Config["ConnectionStrings:DatabaseConnectionString"]));               

            return services;
        }
    }
}
