using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.LotteryDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.LotteryCommands
{
    public class GetDetailedRecordsCommand : IRequest<IEnumerable<LotteryRecordsForManagerResponse>>
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
        public string Channel { get; set; }

        /// <summary>
        /// 活动号
        /// </summary>
        /// <value></value>
        [Required]
        public string ActivityCode { get; set; }
    }
}
