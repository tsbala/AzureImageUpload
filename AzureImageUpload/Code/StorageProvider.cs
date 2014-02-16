using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace ImageUploadAPI.Code
{
    internal static class StorageProvider
    {
        private static readonly CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("CloudStorageConnectionString"));

        public static CloudBlobContainer DefaultContainer()
        {
            var container = StorageAccount.GetBlobContainer("images");

            // Enable public access to blob
            var permissions = container.GetPermissions();
            if (permissions.PublicAccess == BlobContainerPublicAccessType.Off)
            {
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                container.SetPermissions(permissions);
            }

            return container;
        }

        public static CloudQueue DefaultQueueClient()
        {
            return StorageAccount.GetQueueClient("resizequeue");
        }

        private static CloudBlobContainer GetBlobContainer(this CloudStorageAccount cloudStorageAccount, string containerName)
        {
            var client = cloudStorageAccount.CreateCloudBlobClient();
            client.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(5));
            var container = client.GetContainerReference(containerName);
            container.CreateIfNotExist();
            return container;
        }

        private static CloudQueue GetQueueClient(this CloudStorageAccount cloudStorageAccount, string queueName)
        {
            var client = cloudStorageAccount.CreateCloudQueueClient();
            var queueClient = client.GetQueueReference(queueName);
            queueClient.CreateIfNotExist();
            return queueClient;
        }
    }
}