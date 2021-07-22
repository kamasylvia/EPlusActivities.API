using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.AttendanceDtos
{
    public class AttendanceDto
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        [Required]
        public Guid? UserId { get; set; }
    }
}
