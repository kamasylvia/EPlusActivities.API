using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    [CustomDependency(ServiceLifetime.Scoped)]
    public class LotteryDetailRepository : RepositoryBase<LotteryDetail>, ILotteryDetailRepository
    {
        public LotteryDetailRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.LotteryResults
                .AsAsyncQueryable()
                .AnyAsync(lr => lr.Id == (Guid)keyValues.FirstOrDefault());

        public override async Task<LotteryDetail> FindByIdAsync(params object[] keyValues) =>
            await _context.LotteryResults
                .Include(lottery => lottery.Activity)
                .Include(lr => lr.User)
                .Include(lr => lr.PrizeTier)
                .Include(lr => lr.PrizeItem)
                .SingleOrDefaultAsync(lottery => lottery.Id == (Guid)keyValues.FirstOrDefault());

        public async Task<IEnumerable<LotteryDetail>> FindByUserIdAsync(Guid userId) =>
            await _context.LotteryResults
                .Include(lr => lr.User)
                .Include(lr => lr.Activity)
                .Include(lr => lr.PrizeTier)
                .Include(lr => lr.PrizeItem)
                .AsAsyncEnumerable()
                .Where(a => a.User.Id == userId)
                .ToListAsync();

        public async Task<IEnumerable<LotteryDetail>> FindByActivityIdAsync(Guid activityId) =>
            await _context.LotteryResults
                .Include(lr => lr.User)
                .Include(lr => lr.Activity)
                .Include(lr => lr.PrizeTier)
                .Include(lr => lr.PrizeItem)
                .AsAsyncEnumerable()
                .Where(a => a.Activity.Id == activityId)
                .ToListAsync();
    }
}
