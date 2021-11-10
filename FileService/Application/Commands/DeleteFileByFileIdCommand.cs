using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.Grpc.Messages.FileService;
using MediatR;

namespace FileService.Application.Commands
{
    public class DeleteFileByFileIdCommand : IRequest<DeleteFileGrpcResponse>
    {
        public DeleteFileByFileIdGrpcRequest GrpcRequest { get; set; }
    }
}
