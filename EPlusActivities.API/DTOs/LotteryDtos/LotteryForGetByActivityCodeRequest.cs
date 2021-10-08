using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlusActivities.API.Dtos.LotteryDtos
{
    public class LotteryForGetByActivityCodeRequest
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        /// <value></value>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        /// <value></value>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        /// <value></value>
        public string Channel { get; set; }

        /// <summary>
        /// 活动号
        /// </summary>
        /// <value></value>
        public string ActivityCode { get; set; }
    }
}
