using System.IO;
using System.Threading.Tasks;
using FileService.Dtos.FileDtos;
using Microsoft.AspNetCore.Http;

namespace FileService.Services.FileStorageService
{
    public interface IFileStorageService
    {
        Task<bool> UploadFileAsync(UploadFileRequestDto fileDto);

        Task<MemoryStream> DownloadFileAsync(string filePath);

        bool DeleteFile(string filePath);
    }
}
