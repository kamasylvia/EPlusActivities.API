using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IGeneralLotteryRecordsRepository : IRepository<GeneralLotteryRecords>
    {
        Task<IEnumerable<GeneralLotteryRecords>> FindByDateRangeAsync(
            Guid activityId,
            ChannelCode channel,
            DateTime? startTime,
            DateTime? endTime
        );

        Task<GeneralLotteryRecords> FindByDateAsync(
            Guid activityId,
            ChannelCode channel,
            DateTime date
        );
    }
}
