using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Services.LotteryService
{
    public interface ILotteryDrawService
    {
        Task<(PrizeTier, PrizeItem)> DrawPrizeAsync(Activity activity);
    }
}
