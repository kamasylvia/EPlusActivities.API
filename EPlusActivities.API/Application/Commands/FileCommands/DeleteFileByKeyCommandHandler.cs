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
    public class DeleteFileByKeyCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DeleteFileByKeyCommand>
    {
        public DeleteFileByKeyCommandHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<Unit> Handle(
            DeleteFileByKeyCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!await _fileService.DeleteFileByKeyAsync(request))
            {
                throw new RemoteServiceException("Failed to delete the file on the file server.");
            }
            return new Unit();
        }
    }
}
