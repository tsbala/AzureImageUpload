using Microsoft.WindowsAzure;

namespace ImageResizerLib.Configuration
{
    public static class CloudProviderSettings
    {
        public static string Account
        {
            get { return CloudConfigurationManager.GetSetting("CloudStorageConnectionString"); }
        }

        public static string Blob
        {
            get { return "images"; }
        }

        public static string Queue
        {
            get { return "resize"; }
        }
    }
}
