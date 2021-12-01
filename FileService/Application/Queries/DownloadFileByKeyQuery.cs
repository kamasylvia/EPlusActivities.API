using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace FileService.Application.Queries
{
    public class DownloadFileByKeyQuery : IRequest<DownloadFileGrpcResponse>
    {
        public DownloadFileByKeyGrpcRequest GrpcRequest { get; set; }
    }
}
