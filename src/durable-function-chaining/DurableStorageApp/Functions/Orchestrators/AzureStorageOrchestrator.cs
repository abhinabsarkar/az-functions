using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DurableStorageApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

/// <summary>
/// The orchestrator for Azure Durable App 
/// Triggered by BLOB Trigger BlobTriggerStart.cs 
/// Describes the stateful workflow of the orchestration
/// </summary>
namespace DurableStorageApp
{
    public static class AzureStorageOrchestrator
    {
        [FunctionName("AzureStorageOrchestrator")]
        public static async Task<string> RunOrchestrator(
           [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            try
            {
                var uploadedCloudBlob = context.GetInput<CloudBlobItem>();
                bool isEmailSentToAdmin;

                //Chain #1 Send Message with BLOB details to Service Bus Queue. Returns the ServiceBus Message
                var serviceBusMessage = await context.CallActivityAsync<string>("SendMessageToServiceBusQueue", uploadedCloudBlob);

                if (serviceBusMessage != null)
                {                
                    
                    //Chain #2 send email using Azure Communication Service Email API
                    isEmailSentToAdmin = await context.CallActivityAsync<bool>("SendEmailNotification", uploadedCloudBlob);

                    //TODO Chain: #3
                    // Example 1. An activity function that saves the uploaded BLOB to Azure Cosmos DB
                    // Example 2. An activity function that reads and receives the queue message on the Azure Service Bus Queue

                    log.LogInformation($"A new cloud blob named {uploadedCloudBlob.Name} was uploaded to Azure Storage  " +
                        $"and added to service bus queue. \n" +
                        // $" SMS sent = {isSmsSentAndCalledUser} to assigned user. \n" +
                        $" Access via BLOB Name: {uploadedCloudBlob.Name} \n" +
                        $" BLOB size: {uploadedCloudBlob.FileSize} bytes \n" +
                        $" BLOB is processed at:: {uploadedCloudBlob.TimeProcessed}" +
                        $" and email sent.");
                }

                return $"Done with the orchestration with Durable Context Id:  {context.InstanceId}";
            }
            catch (Exception ex)
            {
                //TODO Handle possible errors and do a retry if needed or retry a function
                log.LogError($"Something went wrong " + ex.Message);
                throw;
            }
        }
    }
}