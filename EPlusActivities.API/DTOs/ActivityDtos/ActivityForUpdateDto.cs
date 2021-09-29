using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.ActivityDtos
{
    public class ActivityForUpdateDto
    {
        [Required]
        public Guid? Id { get; set; }

        /// <summary>
        /// 活动名
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// 全活动周期抽奖次数上限，null 表示无限。
        /// </summary>
        /// <value></value>
        public int? Limit { get; set; }

        /// <summary>
        /// 每日抽奖次数上限，null 表示无限。
        /// </summary>
        /// <value></value>
        public int? DailyLimit { get; set; }
        
        /// <summary>
        /// 兑换一次抽奖所需积分，null 表示非抽奖活动
        /// </summary>
        /// <value></value>
        public int? RequiredCreditForRedeeming { get; set; }

        /// <summary>
        /// 允许参加该活动的渠道。字符串不区分大小写。
        /// </summary>
        /// <value>Ngs, NgsPlaza, Alldays</value>
        public IEnumerable<string> AvailableChannels { get; set; }

        /// <summary>
        /// 活动展示类型
        /// </summary>
        /// <value>None, Roulette, Digging, Scratchcard</value>
        public string LotteryDisplay { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        /// <value></value>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        /// <value></value>
        public DateTime? EndTime { get; set; }
    }
}
