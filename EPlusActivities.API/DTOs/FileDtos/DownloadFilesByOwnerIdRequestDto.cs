using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.FileDtos
{
    public class DownloadFilesByOwnerIdRequestDto
    {
        [Required]
        public Guid? OwnerId { get; set; }
    }
}
