using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.DTOs.CategoryDtos
{
    public class CategoryForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
