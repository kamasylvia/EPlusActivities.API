using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.FileDtos;
using EPlusActivities.API.Services.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class DownloadFilesByOwnerIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DownloadFilesByOwnerIdCommand, IEnumerable<DownloadFilesByOwnerIdDto>>
    {
        public DownloadFilesByOwnerIdCommandHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<IEnumerable<DownloadFilesByOwnerIdDto>> Handle(
            DownloadFilesByOwnerIdCommand request,
            CancellationToken cancellationToken
        ) => await _fileService.DownloadFilesByOwnerIdAsync(request.OwnerId.Value);
    }
}
