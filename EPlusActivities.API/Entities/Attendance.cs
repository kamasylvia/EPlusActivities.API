using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class Attendance
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}