using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.CategoryDtos
{
    public record CategoryDto
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
