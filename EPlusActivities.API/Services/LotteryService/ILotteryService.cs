using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Services.LotteryService
{
    public interface ILotteryService
    {
        Task<(PrizeTier, PrizeItem)> DrawPrizeAsync(Activity activity);

    }
}
