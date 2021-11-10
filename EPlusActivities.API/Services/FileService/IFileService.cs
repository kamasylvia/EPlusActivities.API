using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.FileCommands;
using EPlusActivities.Grpc.Messages.FileService;

namespace EPlusActivities.API.Services.FileService
{
    public interface IFileService
    {
        Task<UploadFileGrpcResponse> UploadFileAsync(UploadFileCommand request);

        Task<DeleteFileGrpcResponse> DeleteFileByFileIdAsync(DeleteFileByFileIdCommand request);

        Task<DeleteFileGrpcResponse> DeleteFileByKeyAsync(DeleteFileByKeyCommand request);

        Task<DownloadFileGrpcResponse> DownloadFileByFileIdAsync(
            DownloadFileByFileIdCommand request
        );

        Task<DownloadFileGrpcResponse> DownloadFileByKeyAsync(DownloadFileByKeyCommand request);
    }
}
