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
    public class GeneralLotteryRecordsRepository
        : RepositoryBase<GeneralLotteryRecords>,
          IGeneralLotteryRecordsRepository
    {
        public GeneralLotteryRecordsRepository(ApplicationDbContext context) : base(context) { }

        public async Task<GeneralLotteryRecords> FindByDateAsync(
            Guid activityId,
            ChannelCode channel,
            DateTime date
        ) =>
            await _context.GeneralLotteryRecords
                .Include(s => s.Activity)
                .AsAsyncEnumerable()
                .SingleOrDefaultAsync(
                    s =>
                        s.Activity.Id == activityId
                        && s.Channel == channel
                        && s.DateTime.Date == date.Date
                );

        public async Task<IEnumerable<GeneralLotteryRecords>> FindByDateRangeAsync(
            Guid activityId,
            ChannelCode channel,
            DateTime? startTime,
            DateTime? endTime
        ) =>
            await _context.GeneralLotteryRecords
                .Include(s => s.Activity)
                .AsAsyncEnumerable()
                .Where(
                    s =>
                        s.Activity.Id.Value == activityId
                        && s.Channel == channel
                        && !(startTime > s.DateTime)
                        && !(s.DateTime > endTime)
                )
                .OrderBy(x => x.DateTime)
                .ToListAsync();
    }
}
