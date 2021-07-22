using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class Activity
    {
        public Activity(string name)
        {
            Name = name;
        }

        [Key]
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public IEnumerable<Lottery> LotteryResults { get; set; }

        public IEnumerable<PrizeType> PrizeTypes { get; set; }
    }
}
