using System;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.FileCommands;
using EPlusActivities.API.Application.Queries.FileQueries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 文件、图片 API
    /// </summary>
    [ApiController]
    [Route("choujiang/api/[controller]")]
    public class FileController : Controller
    {
        private readonly IMediator _mediator;

        public FileController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 通过文件 ID 获取文件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("id")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DownloadFileByFileIdAsync(
            [FromQuery] DownloadFileByFileIdQuery request
        )
        {
            var result = await _mediator.Send(request);
            return File(result.Data.ToByteArray(), result.ContentType);
        }
        
        /// <summary>
        /// 通过文件 ID 获取静态文件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("static/id")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DownloadStaticFileByFileIdAsync(
            [FromQuery] DownloadStaticFileByFileIdQuery request
        ) =>
            Ok(await _mediator.Send(request));

        /// <summary>
        /// 通过文件所有者和关键字获取文件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("key")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DownloadFileByKeyAsync(
            [FromQuery] DownloadFileByKeyQuery request
        )
        {
            var result = await _mediator.Send(request);
            return File(result.Data.ToByteArray(), result.ContentType);
        }

        /// <summary>
        /// 通过文件所有者和关键字获取静态文件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("static/key")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DownloadStaticFileByKeyAsync(
            [FromQuery] DownloadStaticFileByKeyQuery request
        ) =>
            Ok(await _mediator.Send(request));

        /// <summary>
        /// 获取某个所有者的全部文件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /*
        [HttpGet("files/ownerId")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<
            ActionResult<IEnumerable<DownloadFilesByOwnerIdDto>>
        > DownloadFilesByOwnerIdAsync([FromQuery] DownloadFilesByOwnerIdCommand request) =>
            Ok(await _mediator.Send(request));
        */

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [DisableRequestSizeLimit]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> UploadFileAsync([FromForm] UploadFileCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        /// <summary>
        /// 根据文件 ID 删除文件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("id")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DeleteFileByIdAsync(
            [FromQuery] DeleteFileByFileIdCommand request
        )
        {
            await _mediator.Send(request);
            return Ok();
        }

        /// <summary>
        /// 根据文件所有者和关键字删除文件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("key")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DeleteFileByKeyAsync(
            [FromQuery] DeleteFileByKeyCommand request
        )
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
