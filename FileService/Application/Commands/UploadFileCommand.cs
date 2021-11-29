using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace FileService.Application.Commands
{
    public class UploadFileCommand : IRequest<UploadFileGrpcResponse>
    {
        public UploadFileGrpcRequest GrpcRequest { get; set; }
    }
}
