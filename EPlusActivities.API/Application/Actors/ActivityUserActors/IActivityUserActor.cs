using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.ActivityUserCommands;
using EPlusActivities.API.Application.Queries.ActivityUserQueries;
using EPlusActivities.API.Application.Queries.UserQueries;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Application.Actors.ActivityUserActors
{
    public interface IActivityUserActor : IActor
    {
        Task BindActivityAndUser(BindActivityAndUserCommand command);
        Task<IEnumerable<ActivityUserDto>> GetActivitiesByUserIdAsync(
            GetActivityUserByUserIdQuery request
        );
        Task<IEnumerable<ActivityUserDto>> BindUserWithAvailableActivitiesAsync(
            BindAvailableActivitiesCommand request
        );
        Task<ActivityUserForRedeemDrawsResponseDto> Redeem(RedeemCommand command);
    }
}
