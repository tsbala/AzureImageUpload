using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ImageUploadAPI.Controllers;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace ImageUploadAPI.Code
{
    public class BlobStorageProvider : MultipartFileStreamProvider
    {
        private readonly CloudBlobContainer _container;
        private readonly CloudQueue _queue;
        public List<FileDetails> Files { get; private set; }

        public BlobStorageProvider(CloudBlobContainer container, CloudQueue queue)
            : base(Path.GetTempPath())
        {
            _container = container;
            _queue = queue;
            Files = new List<FileDetails>();
        }

        public override Task ExecutePostProcessingAsync()
        {
            foreach (var file in FileData)
            {
                var fileName = Path.GetFileName(file.Headers.ContentDisposition.FileName.Trim('"'));
                var directory = _container.GetDirectoryReference(Guid.NewGuid().ToString());
                var blob = directory.GetBlockBlobReference(fileName);

                using (var stream = File.OpenRead(file.LocalFileName))
                {
                    blob.UploadFromStream(stream);
                }


                File.Delete(file.LocalFileName);
                Files.Add(new FileDetails
                {
                    ContentType = blob.Properties.ContentType,
                    Name = blob.Name,
                    Size = blob.Properties.Length,
                    Location = blob.Uri.AbsoluteUri
                }); 
            }

            return base.ExecutePostProcessingAsync();
        }

        private void AddImageResizeMessageToQueue(string directory, string filename, int height, int width)
        {
            var resizeParams = new
                {
                    Directory = directory,
                    Filename = filename,
                    Height = height,
                    Width = width
                };

            var message = new CloudQueueMessage(JsonConvert.SerializeObject(resizeParams));
            _queue.AddMessage(message);
        }
    }
}