using System;
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

namespace FileService.Application.Queries
{
    public class DownloadFileByFileIdCommandHandler
        : HandlerBase,
          IRequestHandler<DownloadFileByFileIdCommand, DownloadFileGrpcResponse>
    {
        public DownloadFileByFileIdCommandHandler(
            IConfiguration configuration,
            IFileStorageService fileStorageService,
            IAppFileRepository fileRepository,
            ILogger<FileStorageService> logger,
            IMapper mapper
        ) : base(configuration, fileStorageService, fileRepository, logger, mapper) { }

        public async Task<DownloadFileGrpcResponse> Handle(
            DownloadFileByFileIdCommand command,
            CancellationToken cancellationToken
        )
        {
            var file = await _fileRepository.FindByIdAsync(Guid.Parse(command.GrpcRequest.FileId));
            if (file is null)
            {
                throw new NotFoundException("Could not find the file.");
            }

            using var memoryStream = await _fileStorageService.DownloadFileAsync(file);
            return new DownloadFileGrpcResponse
            {
                ContentType = file.ContentType,
                Data = ByteString.FromStream(memoryStream)
            };
        }
    }
}
