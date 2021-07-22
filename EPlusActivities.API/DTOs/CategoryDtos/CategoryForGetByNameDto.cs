using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.DTOs.CategoryDtos
{
    public class CategoryForGetByNameDto
    {
        [Required]
        public string Name { get; set; }
    }
}
