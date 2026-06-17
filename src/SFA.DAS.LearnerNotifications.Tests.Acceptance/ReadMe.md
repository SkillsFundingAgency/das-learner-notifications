# Payments V2 Earnings Bridge

The Learner Notifications acceptance tests are responsible for testing the end to end flow of the Learner Notifications function, from receiving messages on the service bus, to storing in the database, and sending out notifications as appropriate.

## How it works

The acceptance tests are built using ReqnRoll, and use the same service bus and database as the main Learner Notifications function.  The tests will send messages to the service bus, and then check the database and any relevant output queues to ensure the correct behaviour is occuring.

## Installation

### Pre-Requisites

Visual Studio 2026 or above

SQL Server or an Azure SQL instance

Azure Service Bus queues.  You will need to have access to the namespace and permissions to create queues for this to work.

### Config

For local running, create a file called 'appSettings.development.json'

Populate as follows:

```
{
  "ConnectionStrings": {
    "StorageConnectionString": "UseDevelopmentStorage=true",
    "ServiceBusConnectionString": "<< Your azure service bus connection string >>",
    "DatabaseConnectionString": "<< You >>"
  },
  "EndpointName": "sfa-das-learnernotifications",
  "UseWebSockets": true,
  "TimeToWait": "00:00:10",
  "TimeToPause": "00:00:02"
}
```
