using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Entities
{
    public class Lottery
    {
        [Key]
        public Guid? Id { get; set; }

        public bool IsLucky { get; set; }

        public bool PickedUp { get; set; }

        public DateTime? PickedUpTime { get; set; }


        [Required]
        public ChannelCode? ChannelCode { get; set; }

        public LotteryCode LotteryCode { get; set; }

        public int UsedCredit { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        public ApplicationUser User { get; set; }

        public Guid? PrizeItemId { get; set; }

        public PrizeItem PrizeItem { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        public Activity Activity { get; set; }

        public Guid? PrizeTypeId { get; set; }

        public PrizeType PrizeType { get; set; }
    }
}
