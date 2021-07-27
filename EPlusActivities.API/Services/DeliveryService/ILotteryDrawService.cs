using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Services.DeliveryService
{
    public interface ILotteryDrawService
    {
        Task<(PrizeTier, PrizeItem)> DrawPrizeAsync(Activity activity);
    }
}
