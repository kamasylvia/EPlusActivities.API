using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace FileService.Application.Queries
{
    public class DownloadFileByFileIdCommand : IRequest<DownloadFileGrpcResponse>
    {
        public DownloadFileByFileIdGrpcRequest GrpcRequest { get; set; }
    }
}
