using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.FileService;
using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Queries.FileQueries
{
    public class DownloadFileByKeyQueryHandler
        : FileRequestHandlerBase,
          IRequestHandler<DownloadFileByKeyQuery, DownloadFileGrpcResponse>
    {
        public DownloadFileByKeyQueryHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<DownloadFileGrpcResponse> Handle(
            DownloadFileByKeyQuery request,
            CancellationToken cancellationToken
        )
        {
            var response = await _fileService.DownloadFileByKeyAsync(request);
            if (response.Data.IsEmpty)
            {
                throw new RemoteServiceException("Could not find the file.");
            }

            return response;
        }
    }
}
