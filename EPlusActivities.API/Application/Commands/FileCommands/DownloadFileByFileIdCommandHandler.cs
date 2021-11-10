using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.FileService;
using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class DownloadFileByFileIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DownloadFileByFileIdCommand, DownloadFileGrpcResponse>
    {
        public DownloadFileByFileIdCommandHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<DownloadFileGrpcResponse> Handle(
            DownloadFileByFileIdCommand request,
            CancellationToken cancellationToken
        )
        {
            var response = await _fileService.DownloadFileByFileIdAsync(request);
            if (response.Data.IsEmpty)
            {
                throw new RemoteServiceException("Could not find the file.");
            }

            return response;
        }
    }
}
