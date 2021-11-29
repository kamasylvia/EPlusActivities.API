using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace FileService.Application.Queries
{
    public class DownloadStaticFileByKeyCommand : IRequest<DownloadStaticFileGrpcResponse>
    {
        public DownloadFileByKeyGrpcRequest GrpcRequest { get; set; }
    }
}
