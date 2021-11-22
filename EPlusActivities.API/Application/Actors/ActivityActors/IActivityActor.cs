using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.ActivityCommands;
using EPlusActivities.API.Application.Commands.ActivityUserCommands;
using EPlusActivities.API.Dtos.ActivityDtos;
using EPlusActivities.API.Dtos.ActivityUserDtos;

namespace EPlusActivities.API.Application.Actors.ActivityActors
{
    public interface IActivityActor : IActor
    {
        Task<ActivityDto> CreateActivity(CreateActivityCommand command);
        Task DeleteActivity(DeleteActivityCommand command);
        Task UpdateActivity(UpdateActivityCommand command);
    }
}
