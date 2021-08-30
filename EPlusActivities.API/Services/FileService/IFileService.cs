using System.IO;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.FileDtos;

namespace EPlusActivities.API.Services.FileService
{
    public interface IFileService
    {
        Task<bool> UploadFileAsync(UploadFileRequestDto uploadPhotoDto);

        Task<string> GetContentTypeByIdAsync(DownloadFileByIdRequestDto downloadedFileDto);

        Task<FileStream> DownloadFileByIdAsync(DownloadFileByIdRequestDto downloadedFileDto);

        Task<string> GetContentTypeByKeyAsync(DownloadFileByKeyRequestDto downloadedFileDto);

        Task<FileStream> DownloadFileByKeyAsync(DownloadFileByKeyRequestDto downloadedFileDto);
    }
}
