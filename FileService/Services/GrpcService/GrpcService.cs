using System;
using System.Threading.Tasks;
using Dapr.Client.Autogen.Grpc.v1;
using EPlusActivities.Grpc.Messages.FileService;
using FileService.Application.Commands;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;

namespace FileService.Services.GrpcService
{
    public class GrpcService : Dapr.AppCallback.Autogen.Grpc.v1.AppCallback.AppCallbackBase
    {
        private readonly IMediator _mediator;

        public GrpcService(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override async Task<InvokeResponse> OnInvoke(
            InvokeRequest request,
            ServerCallContext context
        )
        {
            var response = new InvokeResponse();
            switch (request.Method)
            {
                case "UploadFile":
                    response.Data = Any.Pack(
                        await _mediator.Send(
                            new UploadFileCommand
                            {
                                GrpcRequest = request.Data.Unpack<UploadFileGrpcRequest>()
                            }
                        )
                    );
                    break;
                case "DownloadFileByFileId":
                    response.Data = Any.Pack(
                        await _mediator.Send(
                            new DownloadFileByFileIdCommand
                            {
                                GrpcRequest = request.Data.Unpack<DownloadFileByFileIdGrpcRequest>()
                            }
                        )
                    );
                    break;
                case "DownloadFileByKey":
                    response.Data = Any.Pack(
                        await _mediator.Send(
                            new DownloadFileByKeyCommand
                            {
                                GrpcRequest = request.Data.Unpack<DownloadFileByKeyGrpcRequest>()
                            }
                        )
                    );
                    break;
                case "DeleteFileByFileId":
                    response.Data = Any.Pack(
                        await _mediator.Send(
                            new DeleteFileByFileIdCommand
                            {
                                GrpcRequest = request.Data.Unpack<DeleteFileByFileIdGrpcRequest>()
                            }
                        )
                    );
                    break;
                case "DeleteFileByKey":
                    response.Data = Any.Pack(
                        await _mediator.Send(
                            new DeleteFileByKeyCommand
                            {
                                GrpcRequest = request.Data.Unpack<DeleteFileByKeyGrpcRequest>()
                            }
                        )
                    );
                    break;
                default:
                    break;
            }
            return response;
        }
    }
}
