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

        [HttpGet]
        public async Task<IActionResult> TestAsync()
        {
            System.Console.WriteLine("Hello World");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileAsync([FromForm] UploadFileRequestDto requestDto)
        {
            System.Console.WriteLine("Enter UploadFileAsync");
            return await _fileStorageService.UploadFileAsync(requestDto) ? Ok() : BadRequest();
        }

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

        [HttpGet("content-type/id")]
        public async Task<ActionResult<string>> GetContentTypeByIdAsync(
            [FromQuery] DownloadFileByFileIdRequestDto requestDto
        ) {
            var file = await _appFileRepository.FindByIdAsync(requestDto.FileId);
            return file is null ? NotFound("Could not find the file.") : Ok(file.ContentType);
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

        [HttpGet("content-type/key")]
        public async Task<ActionResult<string>> GetContentTypeByKeyAsync(
            [FromQuery] DownloadFileByOwnerIdRequestDto requestDto
        ) {
            var file = await _appFileRepository.FindByAlternateKeyAsync(
                requestDto.OwnerId.Value,
                requestDto.Key
            );
            return file is null ? NotFound("Could not find the file.") : Ok(file.ContentType);
        }

        [HttpGet("ownerId")]
        public async Task<IEnumerable<Guid?>> GetFileIdsByOwnerIdAsync(
            [FromQuery] GetFileIdsByOwnerIdRequestDto requestDto
        ) {
            var files = await _appFileRepository.FindByOwnerIdAsync(requestDto.OwnerId.Value);
            return files.Count() == 0 ? null : files.Select(file => file.Id);
        }
    }
}
