using System;
using Grpc.Core;

namespace FileService.Infrastructure.Exceptions
{
    public class NotFoundException : RpcException
    {
        public NotFoundException() : base(new Status(StatusCode.NotFound, "Some requested entity (e.g., file or directory) was not found. "))
        {
        }

        public NotFoundException(string message) : base(new Status(StatusCode.NotFound, message))
        {
        }
    }
}
