using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.FileCommands;
using EPlusActivities.API.Application.Queries.FileQueries;
using EPlusActivities.Grpc.Messages.FileService;

namespace EPlusActivities.API.Services.FileService
{
    public interface IFileService
    {
        Task<UploadFileGrpcResponse> UploadFileAsync(UploadFileCommand request);

        Task<DeleteFileGrpcResponse> DeleteFileByFileIdAsync(DeleteFileByFileIdCommand request);

        Task<DeleteFileGrpcResponse> DeleteFileByKeyAsync(DeleteFileByKeyCommand request);

        Task<DownloadFileGrpcResponse> DownloadFileByFileIdAsync(DownloadFileByFileIdQuery request);

        Task<DownloadFileGrpcResponse> DownloadFileByKeyAsync(DownloadFileByKeyQuery request);
    }
}
