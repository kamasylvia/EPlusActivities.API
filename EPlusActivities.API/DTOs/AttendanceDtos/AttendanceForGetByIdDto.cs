using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.AttendanceDtos
{
    public class AttendanceForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
