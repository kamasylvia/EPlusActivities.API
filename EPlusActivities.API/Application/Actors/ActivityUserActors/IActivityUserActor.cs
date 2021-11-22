using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Queries.ActivityUserQueries;
using EPlusActivities.API.Application.Queries.UserQueries;
using EPlusActivities.API.Dtos.ActivityUserDtos;

namespace EPlusActivities.API.Application.Actors.ActivityUserActors
{
    public interface IActivityUserActor : IActor
    {
        Task<IEnumerable<ActivityUserDto>> GetActivitiesByUserIdAsync(
            GetActivityUserByUserIdQuery request
        );
        Task<bool> BindUserWithAvailableActivitiesAsync(LoginQuery request);
    }
}
