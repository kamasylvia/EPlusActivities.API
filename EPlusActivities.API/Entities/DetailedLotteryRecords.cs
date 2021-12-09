using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Entities
{
    public class DetailedLotteryRecords
    {
        [Required]
        [Key]
        public Guid? Id { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public ChannelCode ChannelCode { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual PrizeTier PrizeTier { get; set; }

        public virtual PrizeItem PrizeItem { get; set; }

        public int UsedCredit { get; set; }
    }
}
