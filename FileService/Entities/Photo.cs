using System.ComponentModel.DataAnnotations;

namespace FileService.Entities
{
    public class Photo
    {
        [Required]
        public System.Guid? Id { get; set; }
    }
}
