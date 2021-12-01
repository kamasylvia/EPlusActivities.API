using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace FileService.Application.Queries
{
    public class DownloadStaticFileByFileIdQuery : IRequest<DownloadStaticFileGrpcResponse>
    {
        public DownloadFileByFileIdGrpcRequest GrpcRequest { get; set; }
    }
}
