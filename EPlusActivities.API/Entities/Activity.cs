using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class Activity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        [Required]
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public Guid? LotteryId { get; set; }
        public Lottery Lottery { get; set; }
    }
}