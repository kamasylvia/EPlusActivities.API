using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileService.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileService.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly string _fileStorageDirectory;
        private readonly ILogger<FileService> _logger;
        public FileService(IConfiguration configuration, ILogger<FileService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStorageDirectory = configuration["FileStorageDirectory"];
        }

        public async Task<MemoryStream> DownloadFileAsync(string path, string fileName)
        {
            if (fileName == null || fileName.Length == 0)
                return null;

            var filePath = Path.Combine(path, fileName);
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<bool> UploadFileAsync(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetRandomFileName();
                    var filePath = Path.Combine(_fileStorageDirectory, fileName);
                    // var filePath = Path.GetTempFileName();

                    using (var stream = File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
