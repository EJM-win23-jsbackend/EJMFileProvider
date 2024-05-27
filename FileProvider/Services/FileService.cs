using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Data.Contexts;
using Data.Entities;
using FileProvider.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FileProvider.Services
{
    public class FileService
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<UploadProfileImage> _logger;
        private readonly BlobServiceClient _client;
        private BlobContainerClient _containerClient;

        public FileService(DataContext dataContext, ILogger<UploadProfileImage> logger, BlobServiceClient client)
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

        public string SetFileName(string userId)
        {
            var fileName = userId;
            return fileName;
        }

        public async Task<string> UploadFileAsync(IFormFile file, FileEntity fileEntity)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong when uploading file :: " + ex.Message);
                return null!;
            }
        }

        public async Task SaveToDatabaseAsync(FileEntity fileEntity)
        {
            _dataContext.Files.Add(fileEntity);
            await _dataContext.SaveChangesAsync();

        }          
    }
}
