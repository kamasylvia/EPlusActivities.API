using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Dtos.CategoryDtos
{
    public class CategoryForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
