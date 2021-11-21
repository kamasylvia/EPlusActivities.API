using EPlusActivities.API.Dtos.ActivityDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.ActivityQueries
{
    public class GetActivityByCodeQuery : IRequest<ActivityDto>
    {
        /// <summary>
        /// 活动码
        /// </summary>
        /// <value></value>
        public string ActivityCode { get; set; }
    }
}
