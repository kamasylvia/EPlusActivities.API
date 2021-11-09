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

namespace FileService.Application.Commands
{
    public class DownloadFileByKeyCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DownloadFileByKeyCommand, DownloadFileGrpcResponse>
    {
        public DownloadFileByKeyCommandHandler(
            IConfiguration configuration,
            IFileStorageService fileStorageService,
            IAppFileRepository fileRepository,
            ILogger<FileStorageService> logger,
            IMapper mapper
        ) : base(configuration, fileStorageService, fileRepository, logger, mapper) { }

        public async Task<DownloadFileGrpcResponse> Handle(
            DownloadFileByKeyCommand command,
            CancellationToken cancellationToken
        )
        {
            var file = await _fileRepository.FindByAlternateKeyAsync(
                Guid.Parse(command.GrpcRequest.OwnerId),
                command.GrpcRequest.Key
            );

            if (file is null)
            {
                return null;
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
