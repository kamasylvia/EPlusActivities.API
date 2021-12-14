using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface ILotteryDetailRepository : IRepository<LotteryDetail>
    {
        Task<IEnumerable<LotteryDetail>> FindByUserIdAsync(Guid userId);

        Task<IEnumerable<LotteryDetail>> FindByActivityIdAsync(Guid activityId);

        Task<IEnumerable<LotteryDetail>> FindByDateRangeAsync(
            Guid activityId,
            ChannelCode channel,
            DateTime? startTime,
            DateTime? endTime
        );
    }
}
