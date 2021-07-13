using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public IEnumerable<Prize> Prizes { get; set; }
    }
}