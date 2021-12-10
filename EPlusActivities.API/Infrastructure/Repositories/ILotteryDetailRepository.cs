using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface ILotteryDetailRepository : IRepository<LotteryDetail>
    {
        Task<IEnumerable<LotteryDetail>> FindByUserIdAsync(Guid userId);

        Task<IEnumerable<LotteryDetail>> FindByActivityIdAsync(Guid activityId);
    }
}
