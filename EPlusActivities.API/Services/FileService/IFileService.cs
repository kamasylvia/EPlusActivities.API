using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.FileCommands;
using EPlusActivities.API.Dtos.FileDtos;
using EPlusActivities.Grpc.Messages.FileService;

namespace EPlusActivities.API.Services.FileService
{
    public interface IFileService
    {
        Task<bool> UploadFileAsync(UploadFileCommand request);

        Task<bool> DeleteFileByIdAsync(DeleteFileByIdCommand request);

        Task<bool> DeleteFileByKeyAsync(DeleteFileByKeyCommand request);

        Task<string> GetContentTypeByIdAsync(DownloadFileByIdCommand request);

        Task<DownloadFileGrpcResponse> DownloadFileByIdAsync(DownloadFileByIdCommand request);

        Task<string> GetContentTypeByKeyAsync(DownloadFileByKeyCommand request);

        Task<DownloadFileGrpcResponse> DownloadFileByKeyAsync(DownloadFileByKeyCommand request);

        Task<IEnumerable<DownloadFilesByOwnerIdDto>> DownloadFilesByOwnerIdAsync(Guid ownerId);
    }
}
