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
    public class DeleteFileByFileIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DeleteFileByFileIdCommand>
    {
        public DeleteFileByFileIdCommandHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<Unit> Handle(
            DeleteFileByFileIdCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!(await _fileService.DeleteFileByFileIdAsync(request)).Succeeded)
            {
                throw new RemoteServiceException("Failed to delete the file on the file server.");
            }
            return Unit.Value;
        }
    }
}
