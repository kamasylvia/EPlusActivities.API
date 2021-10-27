using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.ActivityUserDtos;
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
        public string Channel { get; set; }

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
