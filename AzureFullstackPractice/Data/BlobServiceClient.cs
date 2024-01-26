using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace AzureFullstackPractice.Data
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _client;

        public BlobStorageService(BlobServiceClient client)
        {
            _client = client;
        }

        public async Task UploadFileAsync(string containerName, string filePath)
        {
            var blobContainer = _client.GetBlobContainerClient(containerName);
            await blobContainer.CreateIfNotExistsAsync();
            var blobClient = blobContainer.GetBlobClient(Path.GetFileName(filePath));

            await blobClient.UploadAsync(filePath, true);
        }

        public async Task DownloadFileAsync(string containerName, string blobName, string downloadFilePath)
        {
            var blobContainer = _client.GetBlobContainerClient(containerName);
            var blobClient = blobContainer.GetBlobClient(blobName);

            Console.WriteLine($"\nDownloading blob '{blobName}' to\n\t{downloadFilePath}\n");
            Directory.CreateDirectory(Path.GetDirectoryName(downloadFilePath));

            await blobClient.DownloadToAsync(downloadFilePath);
        }


    }
}
