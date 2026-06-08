using Azure.Monitor.OpenTelemetry.Exporter;
using NServiceBus; 
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using SFA.DAS.LearnerNotifications.Application.Data;
using Microsoft.EntityFrameworkCore;


//[assembly: NServiceBusTriggerFunction(endpointName: "sfa-das-learnernotifications", Connection = "ServiceBusConnectionString")]

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<ILearnerNotificationsDataContext, LearnerNotificationsDataContext>(options => { 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnectionString"), sqlOptions => sqlOptions.CommandTimeout(600));
});


builder.Services.AddScoped<INotificationProcessor, NotificationProcessor>();

//builder.AddNServiceBus(config => { 
//    config.Transport.UseWebSockets = builder.Configuration["UseWebSockets"]?.ToLower() == "true";
//    config.AdvancedConfiguration.SendFailedMessagesTo("sfa-das-learnernotifications-errors");
//    config.AdvancedConfiguration.EnableInstallers();    
//});

var appInsightsCnn =  builder.Configuration["AzureMonitor:ConnectionString"];


//builder.Services.AddOpenTelemetry()
//    .UseFunctionsWorkerDefaults()
//    .UseAzureMonitorExporter(options => options.ConnectionString = appInsightsCnn);

await builder.Build().RunAsync();
