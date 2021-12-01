using System;
using System.Threading;
using System.Threading.Tasks;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.FileService;
using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Queries.FileQueries
{
    public class DownloadStaticFileByFileIdQueryHandler
        : IRequestHandler<DownloadStaticFileByFileIdQuery, DownloadStaticFileGrpcResponse>
    {
        private readonly IFileService _fileService;

        public DownloadStaticFileByFileIdQueryHandler(IFileService fileService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public async Task<DownloadStaticFileGrpcResponse> Handle(
            DownloadStaticFileByFileIdQuery query,
            CancellationToken cancellationToken
        )
        {
            var response = await _fileService.DownloadStaticFileByFileIdAsync(query);
            if (string.IsNullOrEmpty(response.Url))
            {
                throw new RemoteServiceException("Could not find the file.");
            }

            return response;
        }
    }
}
