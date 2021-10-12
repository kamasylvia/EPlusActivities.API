using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlusActivities.API.Dtos.LotteryDtos
{
    public class LotteryForGetGeneralRecordsResponse
    {
        /// <summary>
        /// 日期
        /// </summary>
        /// <value></value>
        public DateTime DateTime { get; set; }

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
        /// /// </summary>
        /// <value></value>
        public int Redemption { get; set; }
    }
}
