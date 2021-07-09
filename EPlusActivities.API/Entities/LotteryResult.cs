using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class LotteryResult
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid WinnerId { get; set; }
        public ApplicationUser Winner { get; set; }

        [Required]
        public Guid PrizeId { get; set; }
        public Prize PrizeItem { get; set; }

        [Required]
        public Guid ActivityId { get; set; }
        public Activity ActivityItem { get; set; }
    }
}