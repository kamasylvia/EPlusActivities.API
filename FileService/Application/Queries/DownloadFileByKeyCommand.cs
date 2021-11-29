using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace FileService.Application.Queries
{
    public class DownloadFileByKeyCommand : IRequest<DownloadFileGrpcResponse>
    {
        public DownloadFileByKeyGrpcRequest GrpcRequest { get; set; }
    }
}
