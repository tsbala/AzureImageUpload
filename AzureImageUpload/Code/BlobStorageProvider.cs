using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ImageResizer;
using ImageResizerLib.DTO;
using ImageUploadAPI.Controllers;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace ImageUploadAPI.Code
{
    public class BlobStorageProvider : MultipartFileStreamProvider
    {
        private readonly CloudBlobContainer _container;
        public List<ImageDetails> Images { get; private set; }

        public BlobStorageProvider(CloudBlobContainer container)
            : base(Path.GetTempPath())
        {
            _container = container;
            Images = new List<ImageDetails>();
        }

        public override Task ExecutePostProcessingAsync()
        {
            foreach (var file in FileData)
            {
                var fileName = Path.GetFileName(file.Headers.ContentDisposition.FileName.Trim('"'));
                var identifier = Guid.NewGuid().ToString();
                var directory = _container.GetDirectoryReference(identifier);
                var blob = directory.GetBlockBlobReference(fileName);

                using (var stream = File.OpenRead(file.LocalFileName))
                {
                    blob.UploadFromStream(stream);
                }


                File.Delete(file.LocalFileName);
                Images.Add(new ImageDetails
                {
                    Name = fileName,
                    Size = blob.Properties.Length,
                    Location = blob.Uri.AbsoluteUri,
                    Identifier = identifier
                }); 
            }

            return base.ExecutePostProcessingAsync();
        }
    }
}