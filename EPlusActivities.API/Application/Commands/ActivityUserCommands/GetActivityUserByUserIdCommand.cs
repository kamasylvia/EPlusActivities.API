using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Infrastructure.Enums;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class GetActivityUserByUserIdCommand : IRequest<IEnumerable<ActivityUserDto>>
    {
        [Required]
        public Guid? UserId { get; set; }

        /// <summary>
        /// 用户访问的渠道，字符串不区分大小写。
        /// 取值范围：Ngs, NgsPlaza, Alldays
        /// </summary>
        /// <value></value>
        [Required]
        [EnumDataType(typeof(ChannelCode))]
        public ChannelCode AvailableChannel { get; set; }
    }
}
