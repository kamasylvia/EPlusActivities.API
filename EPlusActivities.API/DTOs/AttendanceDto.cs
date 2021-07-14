using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs
{
    public class AttendanceDto
    {
        public Guid Id { get; set; }

        public DateTime? Date { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}
