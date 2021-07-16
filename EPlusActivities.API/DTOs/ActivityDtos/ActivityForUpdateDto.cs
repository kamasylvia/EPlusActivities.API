using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.ActivityDtos
{
    public class ActivityForUpdateDto
    {
        [Required]
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
