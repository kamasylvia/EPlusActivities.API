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
    [Repository(ServiceLifetime.Scoped)]
    public class LotteryRepository : RepositoryBase<Lottery>, ILotteryRepository
    {
        public LotteryRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.LotteryResults
                .AsAsyncQueryable()
                .AnyAsync(lr => lr.Id == (Guid)keyValues.FirstOrDefault());

        public override async Task<Lottery> FindByIdAsync(params object[] keyValues) =>
            await _context.LotteryResults
                .Include(lottery => lottery.Activity)
                .Include(lr => lr.User)
                .Include(lr => lr.PrizeTier)
                .Include(lr => lr.PrizeItem)
                .SingleOrDefaultAsync(lottery => lottery.Id == (Guid)keyValues.FirstOrDefault());

        public async Task<IEnumerable<Lottery>> FindByUserIdAsync(Guid userId) =>
            await _context.LotteryResults
                .Include(lr => lr.User)
                .Include(lr => lr.Activity)
                .Include(lr => lr.PrizeTier)
                .Include(lr => lr.PrizeItem)
                .AsAsyncEnumerable()
                .Where(a => a.User.Id == userId)
                .ToListAsync();

        public async Task<IEnumerable<Lottery>> FindByActivityIdAsync(Guid activityId) =>
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
