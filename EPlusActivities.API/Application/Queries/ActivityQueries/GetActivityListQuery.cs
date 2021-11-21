using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.ActivityDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.ActivityQueries
{
    public class GetActivityListQuery : IRequest<IEnumerable<ActivityDto>>
    {
        /// <summary>
        /// 是否正在进行
        /// </summary>
        /// <value></value>
        public bool IsAvailable { get; set; }

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
