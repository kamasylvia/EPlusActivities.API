using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface ILotterySummaryRepository : IRepository<LotterySummary>
    {
        Task<IEnumerable<LotterySummary>> FindByDateRangeAsync(
            Guid activityId,
            ChannelCode channel,
            DateOnly? startDate,
            DateOnly? endDate
        );

        Task<LotterySummary> FindByDateAsync(
            Guid activityId,
            ChannelCode channel,
            DateOnly date
        );
    }
}
