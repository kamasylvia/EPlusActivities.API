using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class UploadFileCommandHandler
        : FileRequestHandlerBase,
          IRequestHandler<UploadFileCommand>
    {
        public UploadFileCommandHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<Unit> Handle(
            UploadFileCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!(await _fileService.UploadFileAsync(request)).Succeeded)
            {
                throw new RemoteServiceException("Failed to upload the file to the file server.");
            }

            return Unit.Value;
        }
    }
}
