using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.ActivityUserCommands;
using EPlusActivities.API.Application.Commands.UserCommands;
using EPlusActivities.API.Dtos.ActivityUserDtos;

namespace EPlusActivities.API.Actors
{
    public interface IActivityUserActor : IActor
    {
        Task<IEnumerable<ActivityUserDto>> GetActivitiesByUserIdAsync(
            GetActivityUserByUserIdCommand request
        );
        Task<bool> BindUserWithAvailableActivitiesAsync(LoginCommand request);
    }
}
