using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Elf.WebAPI.Attributes;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Services.LotteryService
{
    [AutomaticDependencyInjection]
    public class LotteryService : ILotteryService
    {
        private readonly IConfiguration _configuration;
        private readonly IPrizeItemRepository _prizeItemRepository;
        private readonly IMapper _mapper;

        public LotteryService(
            IConfiguration configuration,
            IPrizeItemRepository prizeItemRepository,
            IMapper mapper
        )
        {
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
            _prizeItemRepository =
                prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            var total = 0.0;
            var random = new Random();
            var flag = random.Next(100);
            var prizeTiers = activity.PrizeTiers;

            PrizeTier prizeTier = null;
            foreach (var item in prizeTiers)
            {
                // 如果上次中奖日期早于今天，今日中奖人数清零
                if (item.LastDate < DateTime.Today)
                    item.TodayWinnerCount = 0;

                total += item.Percentage;
                if (total > flag && !(item.TodayWinnerCount >= item.DailyLimit))
                {
                    prizeTier = item;

                    // 提取该档包含多奖品列表
                    var prizeItems = (
                        await _prizeItemRepository.FindByPrizeTierIdAsync(prizeTier.Id.Value)
                    ).Where(item => !(item.Stock <= 0));

                    // 如果该档奖品全部没有库存，顺延到下一档
                    if (prizeItems.Count() <= 0)
                        continue;

                    // 在奖品档次中包含的奖品中抽选一件奖品
                    var prizeItem = prizeItems.ElementAtOrDefault(random.Next(prizeItems.Count()));
                    item.TodayWinnerCount++;

                    // 更新上次中奖日期
                    if (item.LastDate < DateTime.Today)
                        item.LastDate = DateTime.Today;

                    // 总库存减一
                    prizeItem.Stock--;

                    return (prizeTier, prizeItem);
                }
            }

            return (null, null);
        }
    }
}
