using ImageResizerLib.DTO;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace ImageUploadAPI.Code
{
    public static class CloudQueueExtensions
    {
        public static void AddStandardResizeRequestsToQueue(this CloudQueue queue, ImageDetails imageDetails)
        {
            queue.AddResizeMessage(new Resize160X160(imageDetails));
            queue.AddResizeMessage(new Resize400X400(imageDetails));
        }

        public static void AddResizeMessage(this CloudQueue queue, ResizeParams resizeParams)
        {
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(resizeParams));
            queue.AddMessage(message);
        }
    }

}