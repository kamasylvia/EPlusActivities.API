using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.ActivityDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.ActivityQueries
{
    public class GetActivityQuery : IRequest<ActivityDto>
    {
        /// <summary>
        /// 活动 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }
    }
}
