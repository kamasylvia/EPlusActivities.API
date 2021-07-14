using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class Lottery
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public bool IsLucky { get; set; }

        [Required]
        public string Channel { get; set; }

        [Required]
        public int UsedCredit { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        public Guid? PrizeId { get; set; }

        public Prize Prize { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        public Activity Activity { get; set; }
    }
}
