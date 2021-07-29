using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.DTOs.AttendanceDtos
{
    public class AttendanceForAttendDto
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        public ChannelCode ChannelCode { get; set; }

        public int EarnedCredits { get; set; }

        public string Reason { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }
    }
}
