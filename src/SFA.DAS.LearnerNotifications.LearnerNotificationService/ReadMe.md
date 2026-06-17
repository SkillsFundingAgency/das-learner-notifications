# Payments V2 Earnings Bridge

The Learner Notifications function is responsible for storing apperentice notifications and sending the push notification if required.

## How it works

The Azure Function listens on a queue in the DAS service bus namespace, stores the earnings messages to a local SQL cache, and as appropriate, will propogate messages out to the relevant Payments V2 applications.

## Installation

### Pre-Requisites

Visual Studio 2022 or above

SQL Server

Azure Service Bus queues.  On start-up the service will try and create the required queue in the DAS service bus namespace, but you will need to have access to the namespace and permissions to create queues for this to work.

### Config

For local running, create a file called 'local.settings.json'

Populate as follows:

```
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "AzureWebJobs.HttpExample.Disabled": "true",        
        "ServiceBusConnectionString": "<< connection string for DAS service bus namespace >>",
        "EndpointName": "sfa-das-learnernotifications",
        "DatabaseConnectionString": "<< SQL Server Connection String for the learner notifications database >>",            
  },
   "Host": {
        "LocalHttpPort": 7071,
        "CORS": "*",
        "CORSCredentials": false
    }
}
```
