using System;
using System.ComponentModel.DataAnnotations;

namespace FileService.Entities
{
    public class AppFile
    {
        [Key]
        public Guid? Id { get; set; }

        [Required]
        public Guid? OwnerId { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string ContentType { get; set; }

        public string FilePath { get; set; }
    }
}
