using Azure.Messaging.ServiceBus;

namespace SFA.DAS.LearnerNotifications.Tests.Acceptance.StepDefinitions
{
    public class MessageSession 
    {
        private ServiceBusClient serviceBusClient;
        private ServiceBusSender serviceBusSender;

        public MessageSession() 
        {
            var serviceBusOptions = new ServiceBusClientOptions
            {
                TransportType = TestRunBindings.Config["UseWebSockets"]?.ToLower() == "true" ? ServiceBusTransportType.AmqpWebSockets : ServiceBusTransportType.AmqpTcp
            };
            serviceBusClient = new ServiceBusClient(TestRunBindings.Config["ConnectionStrings:ServiceBusConnectionString"], serviceBusOptions);
            serviceBusSender = serviceBusClient.CreateSender(TestRunBindings.Config["EndpointName"]);
        }

        public async Task Send<T>(T message)
        {
            var serviceBusMessage = new ServiceBusMessage(System.Text.Json.JsonSerializer.Serialize(message));
            serviceBusMessage.ContentType = "application/json";
            serviceBusMessage.ApplicationProperties.Add("MessageType", typeof(T).FullName);
            serviceBusMessage.ApplicationProperties.Add("NServiceBus.ContentType", "application/json");
            serviceBusMessage.ApplicationProperties.Add("NServiceBus.EnclosedMessageTypes", typeof(T).AssemblyQualifiedName);
            serviceBusMessage.ApplicationProperties.Add("NServiceBus.MessageId", Guid.NewGuid().ToString("D"));
            serviceBusMessage.ApplicationProperties.Add("NServiceBus.CorrelationId", Guid.NewGuid().ToString("D"));
            serviceBusMessage.ApplicationProperties.Add("NServiceBus.MessageIntent", "Send");
            await serviceBusSender.SendMessageAsync(serviceBusMessage);
        }
    }
}
