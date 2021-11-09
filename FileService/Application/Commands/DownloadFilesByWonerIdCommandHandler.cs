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
    public class DownloadFilesByWonerIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DownloadFilesByOwnerIdCommand, DownloadFilesGrpcResponse>
    {
        public DownloadFilesByWonerIdCommandHandler(
            IConfiguration configuration,
            IFileStorageService fileStorageService,
            IAppFileRepository fileRepository,
            ILogger<FileStorageService> logger,
            IMapper mapper
        ) : base(configuration, fileStorageService, fileRepository, logger, mapper) { }

        public async Task<DownloadFilesGrpcResponse> Handle(
            DownloadFilesByOwnerIdCommand command,
            CancellationToken cancellationToken
        )
        {
            var appFiles = await _fileRepository.FindByOwnerIdAsync(
                Guid.Parse(command.GrpcRequest.OwnerId)
            );
            return new DownloadFilesGrpcResponse
            {
                Files =
                {
                    await appFiles
                        .ToAsyncEnumerable()
                        .SelectAwait(
                            async appFile =>
                            {
                                using var memoryStream =
                                    await _fileStorageService.DownloadFileAsync(appFile.FilePath);
                                return new DownloadFileGrpcResponse
                                {
                                    ContentType = appFile.ContentType,
                                    Data = ByteString.FromStream(memoryStream)
                                };
                            }
                        )
                        .ToListAsync()
                }
            };
        }
    }
}
