using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace Studio1BTask.Services
{
    public class ImageUploadService
    {
        private readonly CloudBlobContainer _container;

        public ImageUploadService()
        {
            var connectionString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_Storage") ??
                                   File.ReadAllText("azure-storage-credentials.txt");
            var storageAcc = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAcc.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference("images");
            _container.CreateIfNotExistsAsync().Wait();
        }

        public Task UploadImage(string name, Stream stream)
        {
            var newBlob = _container.GetBlockBlobReference(name);
            newBlob.UploadFromStream(stream);
            newBlob.Properties.ContentType = "image/jpeg";
            newBlob.SetProperties();
            return Task.CompletedTask;
        }
    }
}