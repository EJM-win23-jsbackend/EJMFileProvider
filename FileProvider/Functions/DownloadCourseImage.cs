using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileProvider.Functions
{
    public class DownloadCourseImage
    {
        private readonly ILogger<DownloadCourseImage> _logger;

        public DownloadCourseImage(ILogger<DownloadCourseImage> logger)
        {
            _logger = logger;
        }

        [Function("DownloadCourseImage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "getCourseImage/{courseId}")] HttpRequest req, string courseId)
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureStorageAccount");
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("courseimages");
            BlobClient blobClient = containerClient.GetBlobClient(courseId);

            try
            {
                var downloadFile = await blobClient.DownloadAsync();
                return new FileStreamResult(downloadFile.Value.Content, downloadFile.Value.ContentType)
                {
                    FileDownloadName = courseId
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
