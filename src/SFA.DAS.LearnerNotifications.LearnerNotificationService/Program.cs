using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.LearnerNotifications.Application.Notifications;
using SFA.DAS.LearnerNotifications.Data;
using SFA.DAS.LearnerNotifications.Domain.Configuration;
using SFA.DAS.LearnerNotifications.LearnerNotificationService.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.LearnerNotifications;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();

        var builtConfig = config.Build();
        var environmentName = builtConfig["EnvironmentName"];
        var configNames = builtConfig["ConfigNames"]?.Split(',') ?? new[] { "SFA.DAS.LearnerNotifications" };
        var storageConnectionString = builtConfig["ConfigurationStorageConnectionString"];

        if (!string.IsNullOrEmpty(storageConnectionString) && !string.IsNullOrEmpty(environmentName))
        {
            config.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configNames;
                options.StorageConnectionString = storageConnectionString;
                options.EnvironmentName = environmentName;
                options.PreFixConfigurationKeys = false;
            });
        }
    })
    .ConfigureServices((context, services) =>
    {
        var sqlConnectionString = context.Configuration["LearnerNotifications:SqlConnectionString"]
                                  ?? context.Configuration["DatabaseConnectionString"]
                                  ?? context.Configuration["SqlConnectionString"];

        if (string.IsNullOrEmpty(sqlConnectionString))
            throw new InvalidOperationException("Database connection string not found in configuration");

        services.AddSingleton<TokenCredential>(new DefaultAzureCredential());

        // LOCAL DEVELOPMENT
        //services.AddSingleton<TokenCredential>(new AzureCliCredential());

        services.Configure<LearnerNotificationsConfiguration>(options =>
        {
            options.SqlConnectionString = sqlConnectionString;
        });

        services.AddDbContext<LearnerNotificationsDataContext>();

        services.AddScoped<INotificationService, NotificationService>();
    })
    .Build();

        var configuration = host.Services.GetRequiredService<IConfiguration>();
        var credential = host.Services.GetRequiredService<TokenCredential>();

        var fullyQualifiedFunctionName = configuration["ServiceBus:fullyQualifiedNamespace"];

        if (string.IsNullOrWhiteSpace(fullyQualifiedFunctionName)) throw new InvalidOperationException("Service Bus fully qualified namespace is not configured");

        var serviceBusClient = new ServiceBusAdministrationClient(fullyQualifiedFunctionName, credential);

        var queueName = QueueNames.LearnerNotificationsQueue;
        var queueExists = await serviceBusClient.QueueExistsAsync(QueueNames.LearnerNotificationsQueue);
        
        if (!queueExists)
        {
            await serviceBusClient.CreateQueueAsync(QueueNames.LearnerNotificationsQueue);
        }

        await host.RunAsync();
    }
}
