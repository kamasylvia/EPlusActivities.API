using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.FileDtos;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Services.FileService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;

        public FileController(
            IMapper mapper,
            IFileService fileService,
            ILogger<FileController> logger
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("id")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DownloadFileByIdAsync(
            [FromQuery] DownloadFileByIdRequestDto requestDto
        ) {
            var fileStream = await _fileService.DownloadFileByIdAsync(requestDto);
            if (fileStream is null)
            {
                return NotFound("Could not find the file.");
            }
            var contentType = await _fileService.GetContentTypeByIdAsync(requestDto);

            return File(fileStream, contentType);
        }

        [HttpGet("key")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DownloadFileByKeyAsync(
            [FromQuery] DownloadFileByKeyRequestDto requestDto
        ) {
            var fileStream = await _fileService.DownloadFileByKeyAsync(requestDto);
            if (fileStream is null)
            {
                return NotFound("Could not find the file.");
            }
            var contentType = await _fileService.GetContentTypeByKeyAsync(requestDto);

            return File(fileStream, contentType);
        }

        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> UploadFileAsync(
            [FromForm] UploadFileRequestDto requestDto
        ) =>
            new StatusCodeResult(
                Convert.ToInt32((await _fileService.UploadFileAsync(requestDto)).StatusCode)
            );
    }
}
