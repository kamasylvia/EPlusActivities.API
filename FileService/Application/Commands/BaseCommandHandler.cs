using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FileService.Data.Repositories;
using FileService.Services.FileStorageService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileService.Application.Commands
{
    public abstract class BaseCommandHandler
    {
        protected readonly IConfiguration _configuration;
        protected readonly IFileStorageService _fileStorageService;
        protected readonly IAppFileRepository _fileRepository;
        protected readonly IMapper _mapper;
        protected readonly string _fileStorageDirectory;
        protected BaseCommandHandler(
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
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Directory.CreateDirectory(_fileStorageDirectory);
        }
    }
}