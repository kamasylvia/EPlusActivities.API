using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class PrizeType
    {
        public PrizeType(string name)
        {
            this.Name = name;
        }

        [Key]
        public Guid? Id { get; set; }

        // 几等奖
        public string Name { get; set; }

        // 奖品权重/中奖概率
        public int Percentage { get; set; }

        // 所处活动
        [Required]
        public Activity Activity { get; set; }

        // 单次抽奖所需积分
        public int RequiredCredit { get; set; }

        // 所处的中奖结果
        public IEnumerable<Lottery> LotteryResults { get; set; }

        // 该等第下能兑换的奖品
        public IEnumerable<PrizeTypePrizeItem> PrizeTypePrizeItems { get; set; }
    }
}
