using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.LotteryDtos;
using EPlusActivities.API.Infrastructure.Enums;
using MediatR;

namespace EPlusActivities.API.Application.Queries.LotteryQueries
{
    public class GetDetailedRecordsQuery : IRequest<IEnumerable<LotteryRecordsForManagerResponse>>
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
        [Required]
        [EnumDataType(typeof(ChannelCode))]
        public ChannelCode Channel { get; set; }

        /// <summary>
        /// 活动号
        /// </summary>
        /// <value></value>
        [Required]
        public string ActivityCode { get; set; }
    }
}
