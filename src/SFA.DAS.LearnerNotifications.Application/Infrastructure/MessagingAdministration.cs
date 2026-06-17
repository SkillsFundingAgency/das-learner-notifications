using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LearnerNotifications.Application.Infrastructure
{
    //TODO: Should be treated as tech debt.  Will be replaced by the new AS messaging libraries when they are ready.
    public class MessagingAdministration : IHostedService
    {
        private readonly string endpointName;
        private readonly string connectionString;
        private readonly ILogger<MessagingAdministration> logger;

        public MessagingAdministration(IConfiguration configuration, ILogger<MessagingAdministration> logger)
        {
            endpointName = configuration["EndpointName"] ?? throw new InvalidOperationException("EndpointName is not configured");
            connectionString = configuration["ServiceBusConnectionString"] ?? throw new InvalidOperationException("ServiceBusConnectionString is not configured");
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var adminClient = new ServiceBusAdministrationClient(connectionString);

                await CreateQueue(endpointName, adminClient);
                await CreateQueue($"{endpointName}-errors", adminClient);
            }
            catch
            {
                logger.LogCritical("");
            }
        }

        private async Task CreateQueue(string queueName, ServiceBusAdministrationClient adminClient)
        {
            if (await adminClient.QueueExistsAsync(queueName, CancellationToken.None).ConfigureAwait(false))
            {
                logger.LogInformation($"Queue '{queueName}' already exists, skipping queue creation.");
                return;
            }

            var options = new CreateQueueOptions(queueName)
            {
                DefaultMessageTimeToLive = TimeSpan.FromDays(14),
                DeadLetteringOnMessageExpiration = false,
                LockDuration = TimeSpan.FromMinutes(5),
                MaxDeliveryCount = 10,
                MaxSizeInMegabytes = 5120
            };
            await adminClient.CreateQueueAsync(options, CancellationToken.None).ConfigureAwait(false);
            logger.LogInformation($"Queue '{queueName}' created.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
