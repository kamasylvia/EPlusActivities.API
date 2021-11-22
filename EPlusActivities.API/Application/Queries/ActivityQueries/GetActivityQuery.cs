using System;
using System.ComponentModel.DataAnnotations;
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
