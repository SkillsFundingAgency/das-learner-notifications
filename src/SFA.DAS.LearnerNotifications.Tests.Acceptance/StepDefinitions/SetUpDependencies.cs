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
            var endpointConfig = CreateEndpoint();
            services.AddNServiceBusEndpoint(endpointConfig);
            services.AddSingleton<TestingContext>();

            services.AddDbContext<TestSessionDataContext>(options =>
                options.UseSqlServer(TestRunBindings.Config["ConnectionStrings:DatabaseConnectionString"]));

            return services;
        }

        public static EndpointConfiguration CreateEndpoint(bool sendOnly = false)
        {
            var endpointConfig = new EndpointConfiguration("sfa-das-learnernotifications-specs");
            var conventions = endpointConfig.Conventions();
            conventions.DefiningMessagesAs(type => type.IsMessage());
            endpointConfig.UseSerialization<SystemJsonSerializer>();
            //var storageConnectionString = TestRunBindings.Config["ConnectionStrings:StorageConnectionString"];
            //endpointConfig.UsePersistence<AzureTablePersistence>().ConnectionString(storageConnectionString);
            var connectionString = TestRunBindings.Config["ConnectionStrings:ServiceBusConnectionString"];
            Console.WriteLine($"Config: ConnectionString: {connectionString}");
            var transport = new AzureServiceBusTransport(connectionString, TopicTopology.Default)
            {
                UseWebSockets = TestRunBindings.Config["UseWebSockets"]?.ToLower() == "true"
            };

            endpointConfig.UseTransport(transport);
            endpointConfig.EnableInstallers();
            return endpointConfig;
        }
    }
}
