using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using ImageUploadAPI.Code;
using Microsoft.WindowsAzure.StorageClient;

namespace ImageUploadAPI.Controllers
{
    public class FilesController : ApiController
    {
        private readonly CloudBlobContainer _container;

        public FilesController()
        {
            _container = StorageProvider.DefaultContainer();
        }

        public async Task<IEnumerable<FileDetails>> Post()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new BlobStorageProvider(_container);
            await Request.Content.ReadAsMultipartAsync(provider);
            return provider.Files;
        }
 
        public IEnumerable<FileDetails> Get()
        {
            var container = StorageProvider.DefaultContainer();
            return from CloudBlockBlob blob in container.ListBlobs() select new FileDetails
                {
                    Name = blob.Name,
                    Size = blob.Properties.Length,
                    ContentType = blob.Properties.ContentType,
                    Location = blob.Uri.AbsoluteUri
                };
        }

    }
 
    public class FileDetails
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public string Location { get; set; }
    }
}
