using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.PrizeTierCommands;
using EPlusActivities.API.Dtos.PrizeTierDtos;

namespace EPlusActivities.API.Application.Actors.PrizeTierActors
{
    public interface IPrizeTierActor : IActor
    {
        Task<PrizeTierDto> CreatePrizeTier(CreatePrizeTierCommand command);

        Task DeletePrizeTier(DeletePrizeTierCommand command);

        Task UpdatePrizeTier(UpdatePrizeTierCommand command);
    }
}
