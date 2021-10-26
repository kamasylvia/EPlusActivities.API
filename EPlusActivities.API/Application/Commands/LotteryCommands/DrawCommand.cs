using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.LotteryDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.LotteryCommands
{
    public class DrawCommand : IRequest<IEnumerable<LotteryDto>>
    {
        /// <summary>
        /// 用户访问的渠道，字符串不区分大小写。
        /// 取值范围：Ngs, NgsPlaza, Alldays
        /// </summary>
        /// <value></value>
        [Required]
        public string ChannelCode { get; set; }

        /// <summary>
        /// 活动展示类型，字符串不区分大小写。
        /// 取值范围：None, Roulette, Digging, Scratchcard
        /// </summary>
        /// <value></value>
        [Required]
        public string LotteryDisplay { get; set; }

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
