using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.FileCommands;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.FileService;

namespace EPlusActivities.API.Application.Actors.FileActors
{
    public class FileActor : Actor, IFileActor
    {
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public FileActor(ActorHost host, IMapper mapper, IFileService fileService) : base(host)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public async Task UploadFile(UploadFileCommand command)
        {
            if (!(await _fileService.UploadFileAsync(command)).Succeeded)
            {
                throw new RemoteServiceException("Failed to upload the file to the file server.");
            }
        }
        public async Task DeleteFileByFileId(DeleteFileByFileIdCommand command)
        {
            if (!(await _fileService.DeleteFileByFileIdAsync(command)).Succeeded)
            {
                throw new RemoteServiceException("Failed to delete the file on the file server.");
            }
        }

        public async Task DeleteFileByKey(DeleteFileByKeyCommand command)
        {
            if (!(await _fileService.DeleteFileByKeyAsync(command)).Succeeded)
            {
                throw new RemoteServiceException("Failed to delete the file on the file server.");
            }
        }
    }
}
