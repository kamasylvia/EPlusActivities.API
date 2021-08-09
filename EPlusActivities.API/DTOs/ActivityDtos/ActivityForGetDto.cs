using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.ActivityDtos
{
    public class ActivityForGetDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
