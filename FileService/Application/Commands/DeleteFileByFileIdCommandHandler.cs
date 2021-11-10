using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.Grpc.Messages.FileService;
using FileService.Data.Repositories;
using FileService.Infrastructure.Exceptions;
using FileService.Services.FileStorageService;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileService.Application.Commands
{
    public class DeleteFileByFileIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DeleteFileByFileIdCommand, DeleteFileGrpcResponse>
    {
        public DeleteFileByFileIdCommandHandler(
            IConfiguration configuration,
            IFileStorageService fileStorageService,
            IAppFileRepository fileRepository,
            ILogger<FileStorageService> logger,
            IMapper mapper
        ) : base(configuration, fileStorageService, fileRepository, logger, mapper) { }

        public async Task<DeleteFileGrpcResponse> Handle(
            DeleteFileByFileIdCommand request,
            CancellationToken cancellationToken
        )
        {
            var file = await _fileRepository.FindByIdAsync(Guid.Parse(request.GrpcRequest.FileId));
            if (file is null)
            {
                throw new NotFoundException("Could not find the file.");
            }

            _fileRepository.Remove(file);

            return new DeleteFileGrpcResponse
            {
                Succeeded =
                    _fileStorageService.DeleteFile(file) && await _fileRepository.SaveAsync()
            };
        }
    }
}
