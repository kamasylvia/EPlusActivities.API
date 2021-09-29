using System;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;

namespace EPlusActivities.API.Services.LotteryService
{
    public class LotteryDrawService : ILotteryDrawService
    {
        private readonly IPrizeItemRepository _prizeItemRepository;

        public LotteryDrawService(IPrizeItemRepository prizeItemRepository)
        {
            _prizeItemRepository =
                prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
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
            var prizeTiers = activity.PrizeTiers;

            PrizeTier prizeTier = null;
            foreach (var item in prizeTiers)
            {
                // 如果上次中奖日期早于今天，今日中奖人数清零
                if (item.LastDate < DateTime.Now.Date)
                    item.TodayWinnerCount = 0;

                total += item.Percentage;
                if (
                    total > flag
                    && (!item.DailyLimit.HasValue || item.TodayWinnerCount < item.DailyLimit.Value)
                ) {
                    prizeTier = item;

                    // 提取该档包含多奖品列表
                    var prizeItems = (
                        await _prizeItemRepository.FindByPrizeTierIdAsync(prizeTier.Id.Value)
                    ).Where(item => !item.Stock.HasValue || item.Stock > 0);

                    // 如果该档奖品全部没有库存，顺延到下一档
                    if (prizeItems.Count() <= 0)
                        continue;

                    // 在奖品档次中包含的奖品中抽选一件奖品
                    var prizeItem = prizeItems.ElementAtOrDefault(random.Next(prizeItems.Count()));
                    item.TodayWinnerCount++;

                    // 更新上次中奖日期
                    if (item.LastDate < DateTime.Now.Date)
                        item.LastDate = DateTime.Now.Date;

                    // 总库存减一
                    prizeItem.Stock--;

                    return (
                        prizeTier,
                        prizeItems.ElementAtOrDefault(random.Next(prizeItems.Count()))
                    );
                }
            }

            return (null, null);
        }
    }
}
