using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using FileService.Data.Repositories;
using FileService.Dtos.FileDtos;
using FileService.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileService.Services.FileStorageService
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _fileStorageDirectory;
        private readonly IAppFileRepository _appFileRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FileStorageService> _logger;
        public FileStorageService(
            IConfiguration configuration,
            IMapper mapper,
            ILogger<FileStorageService> logger,
            IAppFileRepository appFileRepository
        ) {
            _appFileRepository =
                appFileRepository ?? throw new ArgumentNullException(nameof(appFileRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStorageDirectory = configuration["FileStorageDirectory"];
            Directory.CreateDirectory(_fileStorageDirectory);
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<MemoryStream> DownloadFileAsync(string filePath)
        {
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<bool> UploadFileAsync(UploadFileRequestDto fileDto)
        {
            try
            {
                var appFile =
                    await _appFileRepository.FindByAlternateKeyAsync(
                        fileDto.OwnerId.Value,
                        fileDto.Key
                    ) ?? _mapper.Map<AppFile>(fileDto);

                var filePath = Path.Combine(_fileStorageDirectory, Path.GetRandomFileName());

                if (File.Exists(appFile.FilePath))
                {
                    File.Delete(appFile.FilePath);
                    appFile.FilePath = filePath;
                    _appFileRepository.Update(appFile);
                }
                else
                {
                    appFile.FilePath = filePath;
                    await _appFileRepository.AddAsync(appFile);
                }

                using (var stream = File.Create(filePath))
                {
                    await fileDto.FormFile.CopyToAsync(stream);
                }

                return await _appFileRepository.SaveAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
