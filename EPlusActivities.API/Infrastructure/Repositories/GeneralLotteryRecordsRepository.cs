using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
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
                .Where(
                    s =>
                        s.Activity.Id == activityId
                        && s.Channel == channel
                        && !(startTime > s.DateTime)
                        && !(s.DateTime > endTime)
                )
                .OrderBy(x => x.DateTime)
                .ToListAsync();
    }
}
