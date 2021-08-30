using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.FileDtos
{
    public class DownloadFileByIdRequestDto
    {
        [Required]
        public Guid? FileId { get; set; }
    }
}
