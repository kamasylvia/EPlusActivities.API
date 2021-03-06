using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.LotteryStatementDtos
{
    public record GetLotteryDetailsResponse
    {
        /// <summary>
        /// 日期
        /// </summary>
        /// <value></value>
        public string Date { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        /// <value></value>
        public string Time { get; set; }

        /// <summary>
        /// 用户手机号
        /// </summary>
        /// <value></value>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 抽奖渠道
        /// </summary>
        /// <value></value>
        [EnumDataType(typeof(ChannelCode))]
        public ChannelCode ChannelCode { get; set; }

        /// <summary>
        /// 抽奖活动号
        /// </summary>
        /// <value></value>
        public string ActivityCode { get; set; }

        /// <summary>
        /// 抽奖活动名称
        /// </summary>
        /// <value></value>
        public string ActivityName { get; set; }

        /// <summary>
        /// 用户 ID
        /// </summary>
        /// <value></value>
        public Guid UserId { get; set; }

        /// <summary>
        /// 中奖名称
        /// </summary>
        /// <value></value>
        public string PrizeTierName { get; set; }

        /// <summary>
        /// 奖励类型
        /// </summary>
        /// <value></value>
        public string PrizeType { get; set; }

        /// <summary>
        /// 中奖内容
        /// </summary>
        /// <value></value>
        public string PrizeContent { get; set; }

        /// <summary>
        /// 消耗积分
        /// </summary>
        /// <value></value>
        public int UsedCredit { get; set; }
    }
}
