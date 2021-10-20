using System;
using System.Collections.Generic;
using System.IO;
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
    /// <summary>
    /// 文件、图片 API
    /// </summary>
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("choujiang/api/[controller]")]
    public class FileController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;

        public FileController(
            IMapper mapper,
            IFileService fileService,
            ILogger<FileController> logger
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 通过文件 ID 获取文件
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [HttpGet("id")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DownloadFileByIdAsync(
            [FromQuery] DownloadFileByIdRequestDto requestDto
        )
        {
            var fileStream = await _fileService.DownloadFileByIdAsync(requestDto);
            if (fileStream.Length == 0)
            {
                return NotFound("Could not find the file.");
            }
            var contentType = await _fileService.GetContentTypeByIdAsync(requestDto);

            return File(fileStream, contentType);
        }

        /// <summary>
        /// 通过文件所有者和关键字获取文件
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [HttpGet("key")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DownloadFileByKeyAsync(
            [FromQuery] DownloadFileByKeyRequestDto requestDto
        )
        {
            var fileStream = new MemoryStream(
                await _fileService.DownloadFileByKeyAsync(requestDto)
            );
            if (fileStream is null)
            {
                return NotFound("Could not find the file.");
            }
            var contentType = await _fileService.GetContentTypeByKeyAsync(requestDto);

            return File(fileStream, contentType);
        }

        /// <summary>
        /// 获取某个所有者的全部文件
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [HttpGet("ownerId")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<Guid>>> DownloadFilesByOwnerIdAsync(
            [FromQuery] DownloadFilesByOwnerIdRequestDto requestDto
        ) => Ok(await _fileService.DownloadFilesByOwnerIdAsync(requestDto.OwnerId.Value));

        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> UploadFileAsync(
            [FromForm] UploadFileRequestDto requestDto
        ) => new StatusCodeResult(await _fileService.UploadFileAsync(requestDto));

        /// <summary>
        /// 删除指定 ID 的文件
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [HttpDelete("id")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DeleteFileByIdAsync(
            [FromQuery] DownloadFileByIdRequestDto requestDto
        ) => new StatusCodeResult(await _fileService.DeleteFileByIdAsync(requestDto));

        /// <summary>
        /// 根据文件所有者和关键字删除文件
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [HttpDelete("key")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DeleteFileByKeyAsync(
            [FromQuery] DownloadFileByKeyRequestDto requestDto
        ) => new StatusCodeResult(await _fileService.DeleteFileByKeyAsync(requestDto));
    }
}
