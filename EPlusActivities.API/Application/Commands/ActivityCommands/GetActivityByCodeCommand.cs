using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.ActivityDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityCommands
{
    public class GetActivityByCodeCommand : IRequest<ActivityDto>
    {
        /// <summary>
        /// 活动码
        /// </summary>
        /// <value></value>
        public string ActivityCode { get; set; }
    }
}
