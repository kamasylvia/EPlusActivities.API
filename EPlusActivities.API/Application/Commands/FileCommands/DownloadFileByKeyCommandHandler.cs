using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.FileDtos;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.FileService;
using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class DownloadFileByKeyCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DownloadFileByKeyCommand, DownloadFileGrpcResponse>
    {
        public DownloadFileByKeyCommandHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<DownloadFileGrpcResponse> Handle(
            DownloadFileByKeyCommand request,
            CancellationToken cancellationToken
        ) => await _fileService.DownloadFileByKeyAsync(request);
    }
}
