using System;

namespace FileService.Dtos.FileDtos
{
    public class DownloadFileRequestDto
    {
        public Guid? FileId { get; set; }
        public string ContentType { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }
}
