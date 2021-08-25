using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileService.Dtos.FileDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        public FileController()
        {
        }

        [HttpPost("file")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file)
        {
            var filePath = Path.GetTempFileName();

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return Ok();
        }

        [HttpPost("files")]
        public async Task<IActionResult> UploadFilesAsync(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var filePath = Path.GetTempFileName();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFileAsync([FromBody] DownloadFileRequestDto requestDto)
        {
            var filePath = Path.Combine(requestDto.FilePath, requestDto.FileName);
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;

            return File(memoryStream, requestDto.ContentType, Path.GetFileName(requestDto.FilePath));
        }
    }
}
