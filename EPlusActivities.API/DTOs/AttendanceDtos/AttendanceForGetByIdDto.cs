using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.AttendanceDtos
{
    public class AttendanceForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
