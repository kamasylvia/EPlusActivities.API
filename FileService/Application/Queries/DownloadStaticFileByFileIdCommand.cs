using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace FileService.Application.Queries
{
    public class DownloadStaticFileByFileIdCommand : IRequest<DownloadStaticFileGrpcResponse>
    {
        public DownloadFileByFileIdGrpcRequest GrpcRequest { get; set; }
    }
}
