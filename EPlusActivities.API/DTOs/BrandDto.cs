using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs
{
    public class BrandDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}