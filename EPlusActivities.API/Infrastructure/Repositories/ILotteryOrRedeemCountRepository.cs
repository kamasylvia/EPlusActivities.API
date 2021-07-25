using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface ILotteryOrRedeemCountRepository
    {
        Task AddAsync(LotteryOrRedeemCount item);

        void Remove(LotteryOrRedeemCount item);

        void Update(LotteryOrRedeemCount item);

        Task<IEnumerable<LotteryOrRedeemCount>> FindAllAsync();

        Task<LotteryOrRedeemCount> FindByIdAsync(Guid userId, Guid activityId);

        Task<bool> SaveAsync();
        // Task<bool> ExistsAsync(Guid userId, Guid activityId);
    }
}
