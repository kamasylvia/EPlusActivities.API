using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.AttendanceDtos
{
    public class AttendanceForGetByUserIdDto
    {
        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
