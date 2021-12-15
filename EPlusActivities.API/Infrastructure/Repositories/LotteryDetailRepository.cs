using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elf.WebAPI.Attributes;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using EPlusActivities.API.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    [AutomaticDependencyInjection(ServiceLifetime.Scoped)]
    public class LotteryDetailRepository : RepositoryBase<LotteryDetail>, ILotteryDetailRepository
    {
        public LotteryDetailRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.LotteryDetails.AnyAsync(lr => lr.Id == (Guid)keyValues.FirstOrDefault());

        public override async Task<LotteryDetail> FindByIdAsync(params object[] keyValues) =>
            await _context.LotteryDetails
                .Include(lottery => lottery.Activity)
                .Include(lr => lr.User)
                .Include(lr => lr.PrizeTier)
                .Include(lr => lr.PrizeItem)
                .SingleOrDefaultAsync(lottery => lottery.Id == (Guid)keyValues.FirstOrDefault());

        public async Task<IEnumerable<LotteryDetail>> FindByUserIdAsync(Guid userId) =>
            await _context.LotteryDetails
                .Include(lr => lr.User)
                .Include(lr => lr.Activity)
                .Include(lr => lr.PrizeTier)
                .Include(lr => lr.PrizeItem)
                .AsAsyncEnumerable()
                .Where(a => a.User.Id == userId)
                .ToListAsync();

        public async Task<IEnumerable<LotteryDetail>> FindByActivityIdAsync(Guid activityId) =>
            await _context.LotteryDetails
                .Include(lr => lr.User)
                .Include(lr => lr.Activity)
                .Include(lr => lr.PrizeTier)
                .Include(lr => lr.PrizeItem)
                .AsAsyncEnumerable()
                .Where(a => a.Activity.Id == activityId)
                .ToListAsync();

        public async Task<IEnumerable<LotteryDetail>> FindByDateRangeAsync(
            Guid activityId,
            ChannelCode channel,
            DateTime? startTime,
            DateTime? endTime
        ) =>
            await _context.LotteryDetails
                .Include(lr => lr.User)
                .Include(lr => lr.Activity)
                .Include(lr => lr.PrizeTier)
                .Include(lr => lr.PrizeItem)
                .AsAsyncEnumerable()
                .Where(
                    ld =>
                        ld.Activity.Id.Value == activityId
                        && ld.ChannelCode == channel
                        && !(startTime > ld.DateTime)
                        && !(ld.DateTime > endTime)
                )
                .ToListAsync();
    }
}
