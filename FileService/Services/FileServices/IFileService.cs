using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileService.Entities;
using Microsoft.AspNetCore.Http;

namespace FileService.Services.FileService
{
    public interface IFileService
    {
        Task<bool> UploadFileAsync(IFormFile file);
        Task<MemoryStream> DownloadFileAsync(string path, string fileName);
    }
}
