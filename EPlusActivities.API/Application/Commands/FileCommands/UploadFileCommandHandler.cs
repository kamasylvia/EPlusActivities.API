using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.FileService;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class UploadFileCommandHandler : BaseCommandHandler, IRequestHandler<UploadFileCommand>
    {
        public UploadFileCommandHandler(IMapper mapper, IFileService fileService)
            : base(mapper, fileService) { }

        public async Task<Unit> Handle(
            UploadFileCommand request,
            CancellationToken cancellationToken
        )
        {
            var mapped = _mapper.Map<Grpc.Messages.FileService.UploadFileGrpcRequest>(request);
            System.Console.WriteLine($"Content-Type = {mapped.ContentType}");
            if (!await _fileService.UploadFileAsync(request))
            {
                throw new RemoteServiceException("Failed to upload the file to the file server.");
            }

            return Unit.Value;
        }
    }
}
