using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class LotteryOrRedeemCount
    {
        // 已 抽奖/兑换 次数
        public int Count { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        public Activity Activity { get; set; }
    }
}
