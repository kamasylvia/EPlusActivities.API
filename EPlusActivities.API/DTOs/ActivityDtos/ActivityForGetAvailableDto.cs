using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.ActivityDtos
{
    public class ActivityForGetAvailableDto
    {
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
        /// 允许参加该活动的渠道，字符串不区分大小写。
        /// 取值范围：Ngs, NgsPlaza, Alldays
        /// </summary>
        /// <value></value>
        public string AvailableChannels { get; set; }
    }
}
