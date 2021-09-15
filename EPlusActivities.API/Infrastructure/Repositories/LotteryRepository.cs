using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class LotteryRepository : RepositoryBase<Lottery>, IFindByParentIdRepository<Lottery>
    {
        public LotteryRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.LotteryResults.AsAsyncEnumerable()
                .AnyAsync(lr => lr.Id == (Guid)keyValues.FirstOrDefault());

        public override async Task<Lottery> FindByIdAsync(params object[] keyValues) =>
            await _context.LotteryResults.Include(lottery => lottery.Activity)
                .Include(lottery => lottery.PrizeItem)
                .SingleOrDefaultAsync(lottery => lottery.Id == (Guid)keyValues.FirstOrDefault());

        public async Task<IEnumerable<Lottery>> FindByParentIdAsync(Guid userId) =>
            await _context.LotteryResults.Include(lr => lr.User)
                .Include(lr => lr.Activity)
                .AsAsyncEnumerable()
                .Where(a => a.User.Id == userId)
                .ToListAsync();
    }
}
