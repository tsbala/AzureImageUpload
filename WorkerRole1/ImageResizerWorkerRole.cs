using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using ImageResizer;
using ImageResizerLib.Configuration;
using ImageResizerLib.DTO;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace WorkerRole1
{
    public class ImageResizerWorkerRole : RoleEntryPoint
    {
        private readonly CloudBlobContainer _blobContainer = StorageProvider.DefaultContainer();

        public override void Run()
        {
            var queue = StorageProvider.DefaultQueueClient();
            while (true)
            {
                var msg = queue.GetMessage();
                if (msg != null)
                {
                    var resizeParams = JsonConvert.DeserializeObject<ResizeParams>(msg.AsString);
                    if (resizeParams != null)
                    {
                        ResizeAndSaveToBlobStorage(resizeParams);
                    }
                    queue.DeleteMessage(msg);
                }
                Trace.TraceInformation("Working", "Information");
            }
        }

        private void ResizeAndSaveToBlobStorage(ResizeParams resizeParams)
        {
            using (var sourceStream = new MemoryStream(DownloadImageBlob(resizeParams.Location)))
            using (var destinationStream = new MemoryStream())
            {
                var instructions = new Instructions { Height = resizeParams.Height, Width = resizeParams.Width };
                var imageJob = new ImageJob(sourceStream, destinationStream, instructions);
                imageJob.Build();

                var directory = _blobContainer.GetDirectoryReference(resizeParams.Identifier + "/" + resizeParams.Height + "/" + resizeParams.Width);
                var blobReference = directory.GetBlockBlobReference(resizeParams.Name);
                destinationStream.Seek(0, SeekOrigin.Begin);
                blobReference.UploadFromStream(destinationStream);
            }
        }

        private byte[] DownloadImageBlob(string downloadImageUrl)
        {
            using (var sourceStream = new MemoryStream())
            {
                var downloadBlobReference = _blobContainer.GetBlockBlobReference(downloadImageUrl);
                downloadBlobReference.DownloadToStream(sourceStream);
                return sourceStream.ToArray();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
