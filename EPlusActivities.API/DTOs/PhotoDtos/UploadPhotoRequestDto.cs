using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EPlusActivities.API.Dtos.PhotoDtos
{
    public class UploadPhotoRequestDto
    {
        [Required]
        public Guid? OwnerId { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public IFormFile FormFile { get; set; }
    }
}
