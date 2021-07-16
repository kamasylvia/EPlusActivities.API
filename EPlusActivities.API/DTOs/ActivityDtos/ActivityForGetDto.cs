using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.ActivityDtos
{
    public class ActivityForGetDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}