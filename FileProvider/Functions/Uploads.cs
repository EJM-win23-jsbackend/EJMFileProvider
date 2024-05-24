using Data.Contexts;
using Data.Entities;
using FileProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileProvider.Functions
{
    public class Uploads
    {
        private readonly ILogger<Uploads> _logger;
        private readonly FileService _fileService;

        public Uploads(ILogger<Uploads> logger, FileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        [Function("Uploads")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                //Form är att dett formulär skickas med http, Files är samling filer som finns i formuläret och ["files"] gör så att vi bara hämtar denna fil från samlingen.
                if (req.Form.Files["file"] is IFormFile file)
                {
                    var containerName = !string.IsNullOrEmpty(req.Query["containerName"]) ? req.Query["containerName"].ToString() : "files";
                    var fileEntity = new FileEntity
                    {
                        FileName = _fileService.SetFileName(file),
                        ContentType = file.ContentType,
                        ContainerName = containerName
                    };

                    await _fileService.SetBlobContainerAsync("profileimages");
                    var filePath = await _fileService.UploadFileAsync(file, fileEntity);
                    fileEntity.FilePath = filePath;

                    await _fileService.SaveToDatabaseAsync(fileEntity);
                    return new OkObjectResult(fileEntity);
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError("Fileprovider.Run :: " + ex.Message);
            }

            return null!;
        }
    }
}
