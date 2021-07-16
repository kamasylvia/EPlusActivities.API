using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.ActivityDtos
{
    public class ActivityForGetAllAvailableDto
    {
        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
