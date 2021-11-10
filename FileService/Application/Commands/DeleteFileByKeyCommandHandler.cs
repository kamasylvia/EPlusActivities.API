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
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileService.Application.Commands
{
    public class DeleteFileByKeyCommandHandler : BaseCommandHandler, IRequestHandler<DeleteFileByKeyCommand, DeleteFileGrpcResponse>
    {
        public DeleteFileByKeyCommandHandler(IConfiguration configuration, IFileStorageService fileStorageService, IAppFileRepository fileRepository, ILogger<FileStorageService> logger, IMapper mapper) : base(configuration, fileStorageService, fileRepository, logger, mapper)
        {
        }

        public async Task<DeleteFileGrpcResponse> Handle(DeleteFileByKeyCommand request, CancellationToken cancellationToken)
        {
            var file = await _fileRepository.FindByAlternateKeyAsync(
               Guid.Parse(request.GrpcRequest.OwnerId),
                request.GrpcRequest.Key
            );
            if (file is null)
            {
                throw new NotFoundException("Could not find the file.");
            }

            _fileRepository.Remove(file);

            return new DeleteFileGrpcResponse { Succeeded = _fileStorageService.DeleteFile(file) && await _fileRepository.SaveAsync() };
        }
    }
}
