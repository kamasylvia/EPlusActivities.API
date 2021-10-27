using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class JoinAvailableActivitiesCommand : IRequest<IEnumerable<ActivityUserDto>>
    {
        [Required]
        public Guid? UserId { get; set; }

        /// <summary>
        /// 用户访问的渠道，字符串不区分大小写。
        /// 取值范围：Ngs, NgsPlaza, Alldays
        /// </summary>
        /// <value></value>
        [Required]
        public string AvailableChannel { get; set; }
    }
}
