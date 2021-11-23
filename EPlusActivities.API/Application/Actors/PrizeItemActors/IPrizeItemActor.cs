using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.PrizeItemCommands;
using EPlusActivities.API.Dtos.PrizeItemDtos;

namespace EPlusActivities.API.Application.Actors.PrizeItemActors
{
    public interface IPrizeItemActor : IActor
    {
        Task<PrizeItemDto> CreatePrizeItem(CreatePrizeItemCommand command);

        Task UpdatePrizeItem(UpdatePrizeItemCommand command);

        Task DeletePrizeItem(DeletePrizeItemCommand command);
    }
}
