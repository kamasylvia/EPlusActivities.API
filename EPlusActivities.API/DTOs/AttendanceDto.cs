using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs
{
    public class AttendanceDto
    {
        [Required]
        public Guid UserId { get; set; }

        public DateTime Date { get; set; }
    }
}