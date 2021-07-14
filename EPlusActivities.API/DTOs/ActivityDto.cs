using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs
{
    public class ActivityDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
