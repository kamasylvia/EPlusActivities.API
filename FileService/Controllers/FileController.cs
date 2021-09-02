using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FileService.Data.Repositories;
using FileService.Dtos.FileDtos;
using FileService.Entities;
using FileService.Infrastructure.ActionResults;
using FileService.Infrastructure.Filters;
using FileService.Services.FileStorageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace FileService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly string _fileStorageDirectory;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        private readonly IAppFileRepository _appFileRepository;
        private readonly ILogger<FileController> _logger;

        public FileController(
            IConfiguration configuration,
            IFileStorageService fileStorageService,
            IMapper mapper,
            ILogger<FileController> logger,
            IAppFileRepository appFileRepository
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appFileRepository =
                appFileRepository ?? throw new ArgumentNullException(nameof(appFileRepository));
            _fileStorageService =
                fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileStorageDirectory = configuration["FileStorageDirectory"];
            Directory.CreateDirectory(_fileStorageDirectory);
        }

        [HttpPost]
        [FileServiceActionFilterAttribute]
        public async Task<IActionResult> UploadFileAsync([FromForm] UploadFileRequestDto requestDto)
        {
            var statusCode = await _fileStorageService.UploadFileAsync(requestDto);
            switch (statusCode)
            {
                case 200:
                    return Ok();
                case 400:
                    return BadRequest("Could not upload an empty file.");
                case 500:
                    return new InternalServerErrorObjectResult("Internal server error.");
                default:
                    return new StatusCodeResult(statusCode);
            }
        }

        [HttpGet("id")]
        public async Task<IActionResult> DownloadFileByFileIdAsync(
            [FromQuery] DownloadFileByFileIdRequestDto requestDto
        )
        {
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
        )
        {
            var file = await _appFileRepository.FindByIdAsync(requestDto.FileId);
            return file is null ? NotFound("Could not find the file.") : Ok(file.ContentType);
        }

        [HttpGet("key")]
        public async Task<IActionResult> DownloadFilesByKeyAsync(
            [FromQuery] DownloadFileByOwnerIdRequestDto requestDto
        )
        {
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
        )
        {
            var file = await _appFileRepository.FindByAlternateKeyAsync(
                requestDto.OwnerId.Value,
                requestDto.Key
            );
            return file is null ? NotFound("Could not find the file.") : Ok(file.ContentType);
        }

        [HttpGet("ownerId")]
        public async Task<IEnumerable<Guid?>> GetFileIdsByOwnerIdAsync(
            [FromQuery] GetFileIdsByOwnerIdRequestDto requestDto
        )
        {
            var files = await _appFileRepository.FindByOwnerIdAsync(requestDto.OwnerId.Value);
            return files.Count() == 0 ? null : files.Select(file => file.Id);
        }
    }
}
