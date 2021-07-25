using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.BrandDtos
{
    public class BrandDto
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
