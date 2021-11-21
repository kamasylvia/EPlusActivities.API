using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class DeleteFileByKeyCommandHandler
        : FileRequestHandlerBase,
          IRequestHandler<DeleteFileByKeyCommand>
    {
        public DeleteFileByKeyCommandHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<Unit> Handle(
            DeleteFileByKeyCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!(await _fileService.DeleteFileByKeyAsync(request)).Succeeded)
            {
                throw new RemoteServiceException("Failed to delete the file on the file server.");
            }
            return Unit.Value;
        }
    }
}
