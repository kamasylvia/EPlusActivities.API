using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.Grpc.Messages.FileService;
using FileService.Data.Repositories;
using FileService.Dtos.FileDtos;
using FileService.Entities;
using FileService.Infrastructure.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FileService.Services.FileStorageService
{
    [CustomDependency(ServiceLifetime.Scoped)]
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
        )
        {
            _appFileRepository =
                appFileRepository ?? throw new ArgumentNullException(nameof(appFileRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStorageDirectory = configuration["FileStorageDirectory"];
            Directory.CreateDirectory(_fileStorageDirectory);
        }

        public bool DeleteFile(AppFile appFile)
        {
            try
            {
                File.Delete(appFile.FilePath);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<MemoryStream> DownloadFileAsync(AppFile appFile)
        {
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(appFile.FilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
