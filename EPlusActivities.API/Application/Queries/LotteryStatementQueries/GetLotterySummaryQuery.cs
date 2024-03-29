﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
using EPlusActivities.API.Infrastructure.Enums;
using MediatR;

namespace EPlusActivities.API.Application.Queries.LotteryStatementQueries
{
    public record GetLotterySummaryStatementQuery : IRequest<IEnumerable<GetLotterySummaryResponse>>
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        /// <value></value>
        public DateOnly? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        /// <value></value>
        public DateOnly? EndDate { get; set; }

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
        [EnumDataType(typeof(ChannelCode))]
        public ChannelCode Channel { get; set; }
    }
}
