using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.DrawingDtos;
using EPlusActivities.API.Infrastructure.Enums;
using MediatR;

namespace EPlusActivities.API.Application.Commands.DrawingCommand
{
    public class DrawCommand : IRequest<IEnumerable<DrawingDto>>
    {
        /// <summary>
        /// 用户访问的渠道，字符串不区分大小写。
        /// 取值范围：Ngs, NgsPlaza, Alldays
        /// </summary>
        /// <value></value>
        [Required]
        [EnumDataType(typeof(ChannelCode))]
        public ChannelCode ChannelCode { get; set; }

        /// <summary>
        /// 活动展示类型，字符串不区分大小写。
        /// 取值范围：None, Roulette, Digging, Scratchcard
        /// </summary>
        /// <value></value>
        [Required]
        [EnumDataType(typeof(LotteryDisplay))]
        public LotteryDisplay LotteryDisplay { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        /// <summary>
        /// 连续抽奖次数
        /// </summary>
        /// <value></value>
        [Range(1, int.MaxValue)]
        public int Count { get; set; }
    }
}
