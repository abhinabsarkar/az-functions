using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using DurableStorageApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
//using Azure.Storage.Blob;
//using Microsoft.WindowsAzure.Storage.Blob;

namespace DurableStorageApp.Functions.Triggers
{
    public static class BlobTriggerStart
    {
        [FunctionName("BlobTriggerStart")]
        public static async Task BlobTriggerClientFunction([BlobTrigger("abhi/{name}", Connection ="StorageConnectionString")] Stream myBlob, string name, 
            ILogger log, [DurableClient] IDurableOrchestrationClient starter)
        {
            try
            {
                log.LogInformation($"Started orchestration trigged by BLOB trigger. A blob item with name = '{name}' \n Size: {myBlob.Length} Bytes");
                log.LogInformation($"BLOB Name {name}");

                // Function input comes from the request content.
                if (myBlob != null)
                {
                    var newUploadedBlobItem = new CloudBlobItem
                    {
                        Name = name,
                        TimeProcessed = DateTime.Now,
                        FileSize = myBlob.Length.ToString()
                    };

                    var instanceId = await starter.StartNewAsync("AzureStorageOrchestrator", newUploadedBlobItem);
                    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
                }
                else
                {
                    log.LogError($"The blob was trigged but myCloudBlob was empty");
                }
            }
            catch (Exception ex)
            {
                //TODO Errorhandling
                log.LogError("Something went wrong. Error : " + ex.InnerException);
                throw;
            }
        }
    }
}
