using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace FileService.Application.Queries
{
    public class DownloadStaticFileByKeyQuery : IRequest<DownloadStaticFileGrpcResponse>
    {
        public DownloadFileByKeyGrpcRequest GrpcRequest { get; set; }
    }
}
