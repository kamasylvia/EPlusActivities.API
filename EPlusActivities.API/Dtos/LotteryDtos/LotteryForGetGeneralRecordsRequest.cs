using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EPlusActivities.API.Dtos.LotteryDtos
{
    public class LotteryForGetGeneralRecordsRequest
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        /// <value></value>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        /// <value></value>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 活动号
        /// </summary>
        /// <value></value>
        [Required]
        public string ActivityCode { get; set; }

        /// <summary>
        /// 渠道号
        /// </summary>
        /// <value></value>
        public string Channel { get; set; }
    }
}
