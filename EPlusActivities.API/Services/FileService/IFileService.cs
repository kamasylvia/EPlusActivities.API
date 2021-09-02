using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.FileDtos;

namespace EPlusActivities.API.Services.FileService
{
    public interface IFileService
    {
        Task<int> UploadFileAsync(UploadFileRequestDto uploadPhotoDto);

        Task<int> DeleteFileByIdAsync(DownloadFileByIdRequestDto requestDto);

        Task<int> DeleteFileByKeyAsync(DownloadFileByKeyRequestDto requestDto);

        Task<string> GetContentTypeByIdAsync(DownloadFileByIdRequestDto downloadedFileDto);

        Task<byte[]> DownloadFileByIdAsync(DownloadFileByIdRequestDto downloadedFileDto);

        Task<string> GetContentTypeByKeyAsync(DownloadFileByKeyRequestDto downloadedFileDto);

        Task<byte[]> DownloadFileByKeyAsync(DownloadFileByKeyRequestDto downloadedFileDto);
    }
}
