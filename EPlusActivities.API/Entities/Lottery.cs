using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class Lottery
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Channel { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Guid? WinnerId { get; set; }

        public ApplicationUser Winner { get; set; }

        [Required]
        public Guid? PrizeId { get; set; }
        public Prize Prize { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }
        public Activity Activity { get; set; }
    }
}