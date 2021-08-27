using System.IO;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.PhotoDtos;

namespace EPlusActivities.API.Services.PhotoService
{
    public interface IPhotoService
    {
        Task<bool> UploadPhotoAsync(UploadPhotoRequestDto uploadPhotoDto);

        Task<FileStream> DownloadFileAsync(DownloadPhotoRequestDto downloadedPhotoDto);
    }
}
