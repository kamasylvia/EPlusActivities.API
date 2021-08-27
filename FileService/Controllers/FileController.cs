using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileService.Data.Repositories;
using FileService.Dtos.FileDtos;
using FileService.Services.FileStorageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace FileService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IAppFileRepository _appFileRepository;
        private readonly ILogger<FileController> _logger;

        public FileController(
            IFileStorageService fileStorageService,
            ILogger<FileController> logger,
            IAppFileRepository appFileRepository
        ) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appFileRepository =
                appFileRepository ?? throw new ArgumentNullException(nameof(appFileRepository));
            _fileStorageService =
                fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileAsync(
            [FromForm] UploadFileRequestDto requestDto
        ) => Ok(await _fileStorageService.UploadFileAsync(requestDto));

        [HttpGet("id")]
        public async Task<IActionResult> DownloadFileByFileIdAsync(
            [FromQuery] DownloadFileByFileIdRequestDto requestDto
        ) {
            var file = await _appFileRepository.FindByIdAsync(requestDto.FileId);
            if (file is null)
            {
                return NotFound("Could not find the file.");
            }

            var memoryStream = await _fileStorageService.DownloadFileAsync(file.FilePath);
            return File(memoryStream, file.ContentType, Path.GetFileName(file.FilePath));
        }

        [HttpGet("key")]
        public async Task<IActionResult> DownloadFilesByKeyAsync(
            [FromQuery] DownloadFileByOwnerIdRequestDto requestDto
        ) {
            var file = await _appFileRepository.FindByAlternateKeyAsync(
                requestDto.OwnerId.Value,
                requestDto.Key
            );
            if (file is null)
            {
                return NotFound("Could not find any file.");
            }

            var memoryStream = await _fileStorageService.DownloadFileAsync(file.FilePath);
            return File(memoryStream, file.ContentType, Path.GetFileName(file.FilePath));
        }

        [HttpGet("files")]
        public async Task<ActionResult<IEnumerable<Guid>>> GetFileIdsByOwnerIdAsync(
            [FromQuery] GetFileIdsByOwnerIdRequestDto requestDto
        ) {
            var files = await _appFileRepository.FindByOwnerIdAsync(requestDto.OwnerId.Value);
            if (files.Count() == 0)
            {
                return NotFound("Could not find any file");
            }

            return Ok(files.Select(file => file.Id));
        }
    }
}
