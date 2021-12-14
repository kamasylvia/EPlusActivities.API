using System;

namespace EPlusActivities.API.Dtos.LotteryStatementDtos
{
    public record GetLotterySummaryResponse
    {
        /// <summary>
        /// 日期
        /// </summary>
        /// <value></value>
        public DateOnly Date { get; set; }

        /// <summary>
        /// 抽奖总数
        /// </summary>
        /// <value></value>
        public int Draws { get; set; }

        /// <summary>
        /// 中奖总数
        /// </summary>
        /// <value></value>
        public int Winners { get; set; }

        /// <summary>
        /// 兑换总数
        /// </summary>
        /// <value></value>
        public int Redemption { get; set; }
    }
}
