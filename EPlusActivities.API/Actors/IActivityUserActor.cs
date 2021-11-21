using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Queries.ActivityUserQueries;
using EPlusActivities.API.Application.Commands.UserCommands;
using EPlusActivities.API.Dtos.ActivityUserDtos;

namespace EPlusActivities.API.Actors
{
    public interface IActivityUserActor : IActor
    {
        Task<IEnumerable<ActivityUserDto>> GetActivitiesByUserIdAsync(
            GetActivityUserByUserIdQuery request
        );
        Task<bool> BindUserWithAvailableActivitiesAsync(LoginCommand request);
    }
}
