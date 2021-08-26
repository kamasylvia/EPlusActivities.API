using System;
using System.ComponentModel.DataAnnotations;

namespace FileService.Dtos.FileDtos
{
    public class DownloadFileByFileIdRequestDto
    {
        [Required]
        public Guid? FileId { get; set; }
    }
}
