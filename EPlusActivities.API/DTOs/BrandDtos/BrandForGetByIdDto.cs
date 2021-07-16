using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.BrandDtos
{
    public class BrandForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
