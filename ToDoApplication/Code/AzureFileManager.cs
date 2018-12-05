using NLog;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ToDoApplication.Code
{
    public class AzureFileManager
    {
        // Retrieve storage account from connection string.
        private CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=accountingdiag182;AccountKey=3bGYmnJUiOHYDoyg+FjMKOf6bLeuwVB9Xg98gwL1h4BeJrMg8tKw4Qu2L52rnkfK9HYHKYoF3SfYtZiUtA3rtg==;EndpointSuffix=core.windows.net");
        private CloudBlobClient blobClient;
        private CloudBlobContainer container;
        private static string containerName = "todo";
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public AzureFileManager()
        {
            // Create the blob client.
            blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve reference to a previously created container.
            container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync();
        }
        
        public async Task<string> UploadFileAsync(byte[] inputData, string keyName)
        {
            var blob = container.GetBlockBlobReference(keyName);
            //blob.Properties.ContentType = "application/json";
            try
            {
                using (Stream stream = new MemoryStream(inputData))
                {
                    blob.UploadFromStream(stream);
                }

                return blob.Uri.AbsoluteUri;
            }
            catch (Exception e)
            {
                logger.Log(NLog.LogLevel.Error, $"Unknown encountered on server. Message:'{e.Message}' when writing an object");
                return e.ToString();
            }

        }
    }
}