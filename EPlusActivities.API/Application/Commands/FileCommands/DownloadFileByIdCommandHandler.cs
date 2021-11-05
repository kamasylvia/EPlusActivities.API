using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.FileService;
using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class DownloadFileByIdCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DownloadFileByIdCommand, DownloadFileGrpcResponse>
    {
        public DownloadFileByIdCommandHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<DownloadFileGrpcResponse> Handle(
            DownloadFileByIdCommand request,
            CancellationToken cancellationToken
        )
        {
            var downloadFileGrpcResponse = await _fileService.DownloadFileByIdAsync(request);
            if (downloadFileGrpcResponse.Data.IsEmpty)
            {
                throw new RemoteServiceException("Could not find the file.");
            }

            return downloadFileGrpcResponse;
        }
    }
}
