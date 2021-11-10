using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace FileService.Application.Commands
{
    public class DownloadFileByFileIdCommand : IRequest<DownloadFileGrpcResponse>
    {
        public DownloadFileByFileIdGrpcRequest GrpcRequest { get; set; }
    }
}
