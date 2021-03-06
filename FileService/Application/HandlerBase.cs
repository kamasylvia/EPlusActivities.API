using System;
using System.IO;
using AutoMapper;
using FileService.Data.Repositories;
using FileService.Services.FileStorageService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileService.Application
{
    public abstract class HandlerBase
    {
        protected readonly IConfiguration _configuration;
        protected readonly IFileStorageService _fileStorageService;
        protected readonly IAppFileRepository _fileRepository;
        private readonly ILogger<FileStorageService> _logger;
        protected readonly IMapper _mapper;
        protected readonly string _fileStorageDirectory;
        protected readonly string _staticStorageDirectory;

        public HandlerBase(
            IConfiguration configuration,
            IFileStorageService fileStorageService,
            IAppFileRepository fileRepository,
            ILogger<FileStorageService> logger,
            IMapper mapper
        )
        {
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fileStorageService =
                fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _fileRepository =
                fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileStorageDirectory = configuration["FileStorageDirectory"];
            Directory.CreateDirectory(_fileStorageDirectory);
            _staticStorageDirectory = "wwwroot/images";
        }
    }
}
