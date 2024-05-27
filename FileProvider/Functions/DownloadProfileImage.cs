using Azure.Storage.Blobs;
using Data.Entities;
using FileProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileProvider.Functions
{
    public class DownloadProfileImage
    {
        private readonly ILogger<DownloadProfileImage> _logger;

        public DownloadProfileImage(ILogger<DownloadProfileImage> logger)
        {
            _logger = logger;
        }

        [Function("Downloads")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "getProfileImage{name}")] HttpRequest req, string name)
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureStorageAccount");
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("profileimages");
            BlobClient blobClient = containerClient.GetBlobClient(name);

            try
            {
                var downloadFile = await blobClient.DownloadAsync();
                return new FileStreamResult(downloadFile.Value.Content, downloadFile.Value.ContentType)
                {
                    FileDownloadName = name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error downloading blob: {ex.Message}");
                return new NotFoundResult();
            }

        }
    }
}
