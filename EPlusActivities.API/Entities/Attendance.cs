using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Entities
{
    public class Attendance
    {
        [Key]
        public Guid? Id { get; set; }

        public ChannelCode ChannelCode { get; set; }

        public int EarnedCredits { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Activity Activity { get; set; }
    }
}
