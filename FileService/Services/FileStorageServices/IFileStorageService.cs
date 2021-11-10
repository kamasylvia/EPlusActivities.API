using System.IO;
using System.Threading.Tasks;
using EPlusActivities.Grpc.Messages.FileService;
using FileService.Dtos.FileDtos;
using FileService.Entities;
using Microsoft.AspNetCore.Http;

namespace FileService.Services.FileStorageService
{
    public interface IFileStorageService
    {
        Task<MemoryStream> DownloadFileAsync(AppFile appFile);

        bool DeleteFile(AppFile appFile);
    }
}
