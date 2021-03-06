using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.Grpc.Messages.FileService;
using FileService.Data.Repositories;
using FileService.Entities;
using FileService.Services.FileStorageService;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileService.Application.Commands
{
    public class UploadFileCommandHandler
        : HandlerBase,
          IRequestHandler<UploadFileCommand, UploadFileGrpcResponse>
    {
        public UploadFileCommandHandler(
            IConfiguration configuration,
            IFileStorageService fileStorageService,
            IAppFileRepository fileRepository,
            ILogger<FileStorageService> logger,
            IMapper mapper
        ) : base(configuration, fileStorageService, fileRepository, logger, mapper) { }

        public async Task<UploadFileGrpcResponse> Handle(
            UploadFileCommand command,
            CancellationToken cancellationToken
        )
        {
            var appFile =
                await _fileRepository.FindByAlternateKeyAsync(
                    Guid.Parse(command.GrpcRequest.OwnerId),
                    command.GrpcRequest.Key
                ) ?? _mapper.Map<AppFile>(command.GrpcRequest);
            var filePath = command.GrpcRequest.IsStatic
                ? Path.Combine(
                      _staticStorageDirectory,
                      Path.GetRandomFileName() + Path.GetExtension(command.GrpcRequest.FileName)
                  )
                : Path.Combine(_fileStorageDirectory, Path.GetRandomFileName());
            if (File.Exists(appFile.FilePath))
            {
                File.Delete(appFile.FilePath);
                appFile.FilePath = filePath;
                _fileRepository.Update(appFile);
            }
            else
            {
                appFile.FilePath = filePath;
                await _fileRepository.AddAsync(appFile);
            }
            using var stream = File.Create(filePath);
            command.GrpcRequest.Content.WriteTo(stream);
            return new UploadFileGrpcResponse { Succeeded = await _fileRepository.SaveAsync() };
        }
    }
}
