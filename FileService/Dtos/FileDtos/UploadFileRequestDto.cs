using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FileService.Dtos.FileDtos
{
    public class UploadFileRequestDto
    {
        [Required]
        public Guid? OwnerId { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public IFormFile FormFile { get; set; }
    }
}
