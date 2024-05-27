using Data.Entities;
using FileProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileProvider.Functions
{
    public class UploadCourseImage
    {
        private readonly ILogger<UploadCourseImage> _logger;
        private readonly FileService _fileService;

        public UploadCourseImage(ILogger<UploadCourseImage> logger, FileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        [Function("UploadCourseImages")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "/uploadcoursepicture/{courseId}")] HttpRequest req, string courseId)
        {
            try
            {
                //Form är att dett formulär skickas med http, Files är samling filer som finns i formuläret och ["files"] gör så att vi bara hämtar denna fil från samlingen.
                if (req.Form.Files["file"] is IFormFile file)
                {
                    var containerName = "courseimages";
                    var fileEntity = new FileEntity
                    {
                        FileName = _fileService.SetFileName(courseId),
                        ContentType = file.ContentType,
                        ContainerName = containerName
                    };

                    await _fileService.SetBlobContainerAsync("courseimages");
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
