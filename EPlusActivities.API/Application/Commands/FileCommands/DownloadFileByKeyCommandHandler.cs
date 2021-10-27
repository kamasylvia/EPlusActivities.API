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
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class DownloadFileByKeyCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DownloadFileByKeyCommand, DownloadFileByKeyDto>
    {
        public DownloadFileByKeyCommandHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<DownloadFileByKeyDto> Handle(
            DownloadFileByKeyCommand request,
            CancellationToken cancellationToken
        )
        {
            throw new NotImplementedException();

            var fileStream = new MemoryStream(await _fileService.DownloadFileByKeyAsync(request));
            if (fileStream is null)
            {
                throw new NotFoundException("Could not find the file.");
            }
            var contentType = await _fileService.GetContentTypeByKeyAsync(request);

            return new DownloadFileByKeyDto { FileStream = fileStream, ContentType = contentType };
        }
    }
}
