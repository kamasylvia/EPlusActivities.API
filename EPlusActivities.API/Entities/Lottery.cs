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

        // 用户是否已经领取奖品
        public bool PickedUp { get; set; }

        // 用户领取奖品日期
        public DateTime? PickedUpTime { get; set; }

        // 抽奖渠道
        [Required]
        public ChannelCode? ChannelCode { get; set; }

        // 抽奖方法
        [Required]
        public LotteryCode? LotteryCode { get; set; }

        // 单次抽奖所需积分
        public int UsedCredit { get; set; }

        // 抽奖日期
        [Required]
        public DateTime? Date { get; set; }

        public ApplicationUser User { get; set; }

        public Activity Activity { get; set; }

        public PrizeItem PrizeItem { get; set; }

        public PrizeTier PrizeTier { get; set; }

        public LotteryOrRedeemCount Count { get; set; }
    }
}
