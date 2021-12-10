using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class PrizeTier
    {
        public PrizeTier(string name)
        {
            this.Name = name;
        }

        [Key]
        public Guid? Id { get; set; }

        // 几等奖
        public string Name { get; set; }

        // 奖品权重/中奖概率，签到活动的概率为 100%
        public double Percentage { get; set; }

        // 每日中奖上限
        public int? DailyLimit { get; set; }

        // 今日中奖人数
        public int TodayWinnerCount { get; set; }

        // 上次中奖时间
        public DateTime LastDate { get; set; }

        // 所处活动
        [Required]
        public virtual Activity Activity { get; set; }

        // 单次抽奖所需积分
        public int RequiredCredit { get; set; }

        // 所处的中奖结果
        public virtual IEnumerable<LotteryDetail> LotteryResults { get; set; }

        // 该等第下能兑换的奖品
        public virtual IEnumerable<PrizeTierPrizeItem> PrizeTierPrizeItems { get; set; }
    }
}
