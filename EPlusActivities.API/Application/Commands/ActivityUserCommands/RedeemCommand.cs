using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Infrastructure.Enums;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class RedeemCommand : IRequest<ActivityUserForRedeemDrawsResponseDto>
    {
        [Required]
        public Guid? ActivityId { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        /// <summary>
        /// 渠道号
        /// </summary>
        /// <value></value>
        [Required]
        [EnumDataType(typeof(ChannelCode))]
        public ChannelCode Channel { get; set; }

        /// <summary>
        /// 兑换几次
        /// </summary>
        /// <value></value>
        public int Count { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        /// <value></value>
        public string Reason { get; set; }
    }
}
