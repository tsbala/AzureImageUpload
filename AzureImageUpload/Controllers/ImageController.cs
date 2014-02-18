using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using ImageResizerLib.DTO;
using ImageResizerLib.Configuration;
using ImageUploadAPI.Code;
using Microsoft.WindowsAzure.StorageClient;

namespace ImageUploadAPI.Controllers
{
    public class ImageController : ApiController
    {
        private readonly CloudBlobContainer _container;
        private readonly CloudQueue _queue;

        public ImageController()
        {
            _container = StorageProvider.DefaultContainer();
            _queue = StorageProvider.DefaultQueueClient();
        }

        [EnableCors("*", "*", "POST")]
        public async Task<IEnumerable<ImageDetails>> Post()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new BlobStorageProvider(_container);
            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var image in provider.Images)
            {
                _queue.AddStandardResizeRequestsToQueue(image);
            }

            return provider.Images;
        }
    }
}
