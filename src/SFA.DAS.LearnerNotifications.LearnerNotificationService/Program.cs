using Azure.Monitor.OpenTelemetry.Exporter;
using NServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;


//[assembly: NServiceBusTriggerFunction("sfa-das-learnernotifications", "ServiceBusConnectionString","TriggerFunction")]

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.AddNServiceBus(config => { 
    //set custom error queue name
});
var appInsightsCnn =  builder.Configuration["AzureMonitor:ConnectionString"];


//builder.Services.AddOpenTelemetry()
//    .UseFunctionsWorkerDefaults()
//    .UseAzureMonitorExporter(options => options.ConnectionString = appInsightsCnn);

builder.Build().Run();
