using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.ActivityDtos
{
    public class ActivityDto
    {
        /// <summary>
        /// 活动 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }

        /// <summary>
        /// 活动名
        /// </summary>
        /// <value></value>
        [Required]
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
        /// 允许参加该活动的渠道，字符串不区分大小写。
        /// 取值范围：Ngs, NgsPlaza, Alldays
        /// </summary>
        /// <value></value>
        public IEnumerable<string> AvailableChannels { get; set; }

        /// <summary>
        /// 活动展示类型，字符串不区分大小写。
        /// 取值范围：None, Roulette, Digging, Scratchcard
        /// </summary>
        /// <value></value>
        public string LotteryDisplay { get; set; }

        /// <summary>
        /// 兑换一次抽奖所需积分，null 表示非抽奖活动
        /// </summary>
        /// <value></value>
        public int? RequiredCreditForRedeeming { get; set; }

        /// <summary>
        /// 活动类型，字符串不区分大小写。
        /// 取值范围：Default, SingleAttendance, SequentialAttendance, Lottery
        /// </summary>
        /// <value></value>
        public string ActivityType { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        /// <value></value>
        [Required]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 活动结束时间，不传或传 null 表示至今。
        /// </summary>
        /// <value></value>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 生成的活动码。
        /// </summary>
        /// <value></value>
        public string ActivityCode { get; set; }
    }
}
