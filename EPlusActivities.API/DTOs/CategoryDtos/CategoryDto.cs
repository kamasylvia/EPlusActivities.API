using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Dtos.CategoryDtos
{
    public class CategoryDto
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
