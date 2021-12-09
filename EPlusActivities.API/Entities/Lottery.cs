using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Entities
{
    public class Lottery
    {
        [Key]
        public Guid? Id { get; set; }

        // 是否中奖
        public bool IsLucky { get; set; }

        // 奖品是否发放
        public bool Delivered { get; set; }

        // 用户是否已经领取奖品
        public bool PickedUp { get; set; }

        // 用户领取奖品日期
        public DateTime? PickedUpTime { get; set; }

        // 抽奖渠道
        public ChannelCode ChannelCode { get; set; }

        // 抽奖方法
        public LotteryDisplay LotteryDisplay { get; set; }

        // 抽奖日期
        [Required]
        public DateTime? DateTime { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual PrizeItem PrizeItem { get; set; }

        public virtual PrizeTier PrizeTier { get; set; }
    }
}
