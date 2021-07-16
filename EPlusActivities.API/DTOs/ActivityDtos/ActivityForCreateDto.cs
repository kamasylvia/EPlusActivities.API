using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.ActivityDtos
{
    public class ActivityForCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
