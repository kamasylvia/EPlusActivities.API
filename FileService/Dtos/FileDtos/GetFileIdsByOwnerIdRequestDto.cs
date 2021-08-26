using System;
using System.ComponentModel.DataAnnotations;

namespace FileService.Dtos.FileDtos
{
    public class GetFileIdsByOwnerIdRequestDto
    {
        [Required]
        public Guid? OwnerId { get; set; }
    }
}
