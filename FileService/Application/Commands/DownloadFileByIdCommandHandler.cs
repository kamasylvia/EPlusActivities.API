using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.Grpc.Messages.FileService;
using FileService.Data.Repositories;
using FileService.Infrastructure.Exceptions;
using FileService.Services.FileStorageService;
using Google.Protobuf;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileService.Application.Commands
{
    public class DownloadFileByIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DownloadFileByIdCommand, DownloadFileGrpcResponse>
    {
        public DownloadFileByIdCommandHandler(
            IConfiguration configuration,
            IFileStorageService fileStorageService,
            IAppFileRepository fileRepository,
            ILogger<FileStorageService> logger,
            IMapper mapper
        ) : base(configuration, fileStorageService, fileRepository, logger, mapper) { }

        public async Task<DownloadFileGrpcResponse> Handle(
            DownloadFileByIdCommand command,
            CancellationToken cancellationToken
        )
        {
            var file = await _fileRepository.FindByIdAsync(command.GrpcRequest.FileId);
            if (file is null)
            {
                throw new NotFoundException("Could not find the file.");
            }

            using var memoryStream = await _fileStorageService.DownloadFileAsync(file.FilePath);
            return new DownloadFileGrpcResponse
            {
                ContentType = file.ContentType,
                Data = ByteString.FromStream(memoryStream)
            };
        }
    }
}
