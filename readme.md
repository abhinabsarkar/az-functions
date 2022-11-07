# Azure Functions

Durable Functions: an extension of [Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview) that lets you write stateful functions in a serverless compute environment. 

You can use [Orchestrator functions](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-orchestrations) (`OrchestrationTrigger`) with Trigger function (e.g., `BlobTrigger`) & [Activity Function](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-types-features-overview#activity-functions) (`ActivityTrigger`) to create a [Function Chaining pattern](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-sequence?tabs=csharp)

The extension lets you define stateful workflows by writing [Orchestrator functions](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-orchestrations) (`OrchestrationTrigger`) and stateful entities by writing [Entity functions](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-entities) using the Azure Functions programming model. 

Behind the scenes, the extension manages state, checkpoints, and restarts for you, allowing you to focus on your business logic.

![alt txt](/images/durable-functions-chaining.jpg)

As a best practice, all the connection strings must be stored on the Azure Key Vault & they can be referenced using Managed Identity of the Azure Function.

![alt txt](/images/akv-reference.jpg)

## Working Sample

Refer the [sample source code](/src/durable-function-chaining/) for implementing the above architecture.

The folder structure shoes below how the code is structured. This is only for visual aid.

```cmd
Tree /A /F
\---Durable-Function-Chaining
    +---DurableStorage.Models
    |   |   CloudBlobItem.cs
    \---DurableStorageApp
        +---Functions
        |   +---Activities
        |   |       SendEmailNotificationToAdmin.cs
        |   |       SendMessageToServiceBusQueue.cs
        |   +---Orchestrators
        |   |       AzureStorageOrchestrator.cs
        |   \---Triggers
        |           BlobTriggerStart.cs
```

### Pre-requisites for running the sample
* Azure Subscription
* Azure Services namely Functions, Blob Storage, Azure Service Bus, Azure Communication Service. You can add/remove any service to have your own integration.
* Azure Function Core Tools to run & test it locally. This sample uses version 4.0
* Dotnet framework. This sample uses version 6.0

On successful execution, it will run as shown below in the local environment:

![alt txt](/images/local-executions.jpg)

## References
* [Hello function - Create a C# function in Azure using Visual Studio Code](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-csharp?tabs=in-process)
* [Create a function in Azure that's triggered by Blob storage](https://learn.microsoft.com/en-us/azure/azure-functions/functions-create-storage-blob-triggered-function)
* [Azure Durable Functions (Function Chaining Example) in C# .NET](https://github.com/jonahandersson/serverless-azure-durable-function-chaining-dotnet-template)
* [Key vault reference](https://learn.microsoft.com/en-us/azure/app-service/app-service-key-vault-references?tabs=azure-cli#reference-syntax)