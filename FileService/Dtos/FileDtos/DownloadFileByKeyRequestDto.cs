using System;
using System.ComponentModel.DataAnnotations;

namespace FileService.Dtos.FileDtos
{
    public class DownloadFileByKeyRequestDto
    {
        [Required]
        public Guid? OwnerId { get; set; }

        [Required]
        public string Key { get; set; }
    }
}
