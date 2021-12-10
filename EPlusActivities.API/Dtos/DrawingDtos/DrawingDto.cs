using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.DrawingDtos
{
    public record DrawingDto
    {
        /// <summary>
        /// 抽奖记录 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }

        /// <summary>
        /// 用户访问的渠道，字符串不区分大小写。
        /// 取值范围：Ngs, NgsPlaza, Alldays
        /// </summary>
        /// <value></value>
        [EnumDataType(typeof(ChannelCode))]
        public ChannelCode ChannelCode { get; set; }

        /// <summary>
        /// 活动展示类型，字符串不区分大小写。
        /// 取值范围：None, Roulette, Digging, Scratchcard
        /// </summary>
        /// <value></value>
        [EnumDataType(typeof(LotteryDisplay))]
        public LotteryDisplay LotteryDisplay { get; set; }

        /// <summary>
        /// 是否中奖
        /// </summary>
        /// <value></value>
        public bool IsLucky { get; set; }

        /// <summary>
        /// 是否已领取奖品
        /// </summary>
        /// <value></value>
        public bool PickedUp { get; set; }

        /// <summary>
        /// 奖品是否发放
        /// </summary>
        /// <value></value>
        public bool Delivered { get; set; }

        /// <summary>
        /// 领取奖品日期，null 表示未领取
        /// </summary>
        /// <value></value>
        public DateTime? PickedUpTime { get; set; }

        /// <summary>
        /// 抽奖日期
        /// </summary>
        /// <value></value>
        public DateTime? DateTime { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        public Guid? PrizeItemId { get; set; }

        public Guid? PrizeTierId { get; set; }
    }
}
