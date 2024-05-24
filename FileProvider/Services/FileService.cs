using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Data.Contexts;
using Data.Entities;
using FileProvider.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FileProvider.Services
{
    public class FileService
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<Uploads> _logger;
        private readonly BlobServiceClient _client;
        private BlobContainerClient _containerClient;

        public FileService(DataContext dataContext, ILogger<Uploads> logger, BlobServiceClient client)
        {
            _dataContext = dataContext;
            _logger = logger;
            _client = client;
        }

        public async Task SetBlobContainerAsync(string containerName)
        {
            _containerClient = _client.GetBlobContainerClient(containerName);

            //Denna del skapar en container om den inte existerar...
            await _containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public string SetFileName(IFormFile file)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            return fileName;
        }

        public async Task<string> UploadFileAsync(IFormFile file, FileEntity fileEntity)
        {
            BlobHttpHeaders headers = new BlobHttpHeaders
            {
                ContentType = file.ContentType,
            };

            var blobClient = _containerClient.GetBlobClient(fileEntity.FileName);

            using var stream = file.OpenReadStream();

            await blobClient.UploadAsync(stream, headers);

            return blobClient.Uri.ToString();
        }

        public async Task SaveToDatabaseAsync(FileEntity fileEntity)
        {
            _dataContext.Files.Add(fileEntity);
            await _dataContext.SaveChangesAsync();

        }
            
    }
}
