using DurableStorageApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DurableStorageApp.Functions.Activities
{
    public class SendEmailNotificationToAdmin
    {
        [FunctionName("SendEmailNotification")]
        public static async Task<bool> SendEmailNotification([ActivityTrigger] CloudBlobItem uploadedBlob, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"BLOB already saved to queue.");

            try
            {
                //Config settings for Azure Service Bus
                var acsEmailAPIConfig = new ConfigurationBuilder()
                     .SetBasePath(executionContext.FunctionAppDirectory)
                     .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables()
                     .Build();

                var sender = acsEmailAPIConfig["ACSEmailSender"];
                var receiver = acsEmailAPIConfig["EmailReceiver"];
                var acsEmailConnectionString = acsEmailAPIConfig["ACSEmailConnectionString"];
                var emailClient = new EmailClient(acsEmailConnectionString);
            

                var subject = "New BLOB Uploaded on Azure Service Bus Queue";
                var emailContent = new EmailContent(subject)
                {
                    Html = @"<p> A new cloud blob file added to Azure Service Bus queue.  BLOB Name:
                                    </a>" + uploadedBlob.Name + "</a><br> Blob is processed at: </a>" + uploadedBlob.TimeProcessed + "</a><br> Blob size: </a>" + uploadedBlob.FileSize + " bytes</a><br> Message from Abhi's app. </p>"               
                };                

                var emailRecipients = new EmailRecipients(new List<EmailAddress> {
                    new EmailAddress(receiver) { DisplayName = "AB" }
                });

                var emailMessage = new EmailMessage(sender, emailContent, emailRecipients);

                // Send email
                SendEmailResult sendEmailResult = await emailClient.SendAsync(emailMessage);                

                // Check if email is sent or not
                string messageId = sendEmailResult.MessageId;
                if (!string.IsNullOrEmpty(messageId))
                {
                    log.LogInformation("Email sent, MessageId = " + messageId);
                    // return true;
                }
                else 
                {
                    log.LogInformation("Failed to send email.");                    
                    return false;
                }

                // wait max 2 minutes to check the send status for mail.
                var cancellationToken = new System.Threading.CancellationTokenSource(TimeSpan.FromMinutes(2));
                do
                {
                    SendStatusResult sendStatus = emailClient.GetSendStatus(messageId);
                    log.LogInformation($"Send mail status for MessageId : <{messageId}>, Status: [{sendStatus.Status}]");

                    if (sendStatus.Status != SendStatus.Queued)
                    {
                        break;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    
                } while (!cancellationToken.IsCancellationRequested);

                if (cancellationToken.IsCancellationRequested)
                {
                    log.LogInformation($"Looks like we timed out for email");
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                //Error handling
                log.LogError($"Receiving Service Bus Queue Message failed and email not sent: {ex.InnerException}");
                throw;
            }
        }
    }
}
