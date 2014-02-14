using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ImageUploadAPI.Controllers;
using Microsoft.WindowsAzure.StorageClient;

namespace ImageUploadAPI.Code
{
    public class BlobStorageProvider : MultipartFileStreamProvider
    {
        private readonly CloudBlobContainer _container;
        public List<FileDetails> Files { get; private set; }

        public BlobStorageProvider(CloudBlobContainer container)
            : base(Path.GetTempPath())
        {
            _container = container;
            Files = new List<FileDetails>();
        }

        public override Task ExecutePostProcessingAsync()
        {
            foreach (var file in FileData)
            {
                var fileName = Path.GetFileName(file.Headers.ContentDisposition.FileName.Trim('"'));
                var directory = _container.GetDirectoryReference("/" + Guid.NewGuid());
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
    }
}