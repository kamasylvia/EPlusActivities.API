using System;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;

namespace EPlusActivities.API.Services.DeliveryService
{
    public class LotteryDrawService : ILotteryDrawService
    {
        private readonly IPrizeItemRepository _prizeItemRepository;

        public LotteryDrawService(IPrizeItemRepository prizeItemRepository)
        {
            _prizeItemRepository = prizeItemRepository
                ?? throw new ArgumentNullException(nameof(prizeItemRepository));
        }

        /// <summary>
        /// 抽奖方法：
        ///     PrizeTier：几等奖。这里是按一档多奖品来写的，一档一奖也可以用，档内随机概率自动上升至 100%。
        ///     PrizeItem：奖品。
        /// </summary>
        /// <param name="activity">抽奖活动</param>
        /// <returns>(几等奖, 奖品)</returns>
        public async Task<(PrizeTier, PrizeItem)> DrawPrizeAsync(Activity activity)
        {
            var total = 0;
            var random = new Random();
            var flag = random.Next(100);
            var prizeTypes = activity.PrizeTiers;

            PrizeTier prizeTier = null;
            foreach (var item in prizeTypes)
            {
                total += item.Percentage;
                if (total > flag)
                {
                    prizeTier = item;
                    break;
                }
            }

            if (prizeTier is null)
            {
                return (null, null);
            }

            var prizeItems = await _prizeItemRepository.FindByPrizeTierIdAsync(prizeTier.Id.Value);
            return (prizeTier, prizeItems.ElementAtOrDefault(random.Next(prizeItems.Count())));
        }
    }
}
