using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using EPlusActivities.API.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    [CustomDependency(ServiceLifetime.Scoped)]
    public class LotterySummaryRepository
        : RepositoryBase<LotterySummary>,
          ILotterySummaryRepository
    {
        public LotterySummaryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<LotterySummary> FindByDateAsync(
            Guid activityId,
            ChannelCode channel,
            DateOnly date
        ) =>
            await _context.LotterySummaryStatement
                .Include(s => s.Activity)
                .AsAsyncEnumerable()
                .SingleOrDefaultAsync(
                    s =>
                        s.Activity.Id == activityId
                        && s.Channel == channel
                        && s.Date == date
                );

        public async Task<IEnumerable<LotterySummary>> FindByDateRangeAsync(
            Guid activityId,
            ChannelCode channel,
            DateOnly? startDate,
            DateOnly? endDate
        ) =>
            await _context.LotterySummaryStatement
                .Include(s => s.Activity)
                .AsAsyncEnumerable()
                .Where(
                    s =>
                        s.Activity.Id.Value == activityId
                        && s.Channel == channel
                        && !(startDate > s.Date)
                        && !(s.Date > endDate)
                )
                .OrderBy(x => x.Date)
                .ToListAsync();
    }
}