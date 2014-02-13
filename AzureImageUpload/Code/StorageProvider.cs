using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace ImageUploadAPI.Code
{
    internal static class StorageProvider
    {
        public static CloudBlobContainer DefaultContainer()
        {
            // Retrieve storage account from connection-string
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("CloudStorageConnectionString"));

            var container = storageAccount.GetBlobContainer("images");
            container.CreateIfNotExist();

            // Enable public access to blob
            var permissions = container.GetPermissions();
            if (permissions.PublicAccess == BlobContainerPublicAccessType.Off)
            {
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                container.SetPermissions(permissions);
            }

            return container;
        }

        private static CloudBlobContainer GetBlobContainer(this CloudStorageAccount storageAccount, string containerName)
        {
            var client = storageAccount.CreateCloudBlobClient();
            client.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(5));
            return client.GetContainerReference(containerName);
        }

    }
}