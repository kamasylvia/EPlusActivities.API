using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.FileService;
using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Queries.FileQueries
{
    public class DownloadFileByFileIdQueryHandler
        : FileRequestHandlerBase,
          IRequestHandler<DownloadFileByFileIdQuery, DownloadFileGrpcResponse>
    {
        public DownloadFileByFileIdQueryHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<DownloadFileGrpcResponse> Handle(
            DownloadFileByFileIdQuery request,
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
