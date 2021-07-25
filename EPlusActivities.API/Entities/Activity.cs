using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

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

        // 抽奖/积分兑换 次数限制，0 表示无限制
        public int Limit { get; set; }

        [Required]
        public string Name { get; set; }

        public ChannelCode ChannelCode { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public IEnumerable<Lottery> LotteryResults { get; set; }

        public IEnumerable<PrizeTier> PrizeTiers { get; set; }
    }
}
