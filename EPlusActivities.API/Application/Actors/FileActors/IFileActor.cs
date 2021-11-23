using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.FileCommands;

namespace EPlusActivities.API.Application.Actors.FileActors
{
    public interface IFileActor : IActor
    {
        Task UploadFile(UploadFileCommand command);
        Task DeleteFileByFileId(DeleteFileByFileIdCommand command);
        Task DeleteFileByKey(DeleteFileByKeyCommand command);
    }
}
