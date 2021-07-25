using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class LotteryOrRedeemCountRepository : RepositoryBase, IRepository<LotteryOrRedeemCount>
    {
        public LotteryOrRedeemCountRepository(ApplicationDbContext context) : base(context) { }

        public async Task AddAsync(LotteryOrRedeemCount item) =>
            await _context.LotteryOrRedeemLimits.AddAsync(item);

        public async Task<bool> ExistsAsync(params Guid[] keyValues) =>
            await _context.LotteryOrRedeemLimits.AnyAsync(
                lorl => lorl.ActivityId.Value == keyValues[0] && lorl.UserId == keyValues[1]
            );

        public async Task<IEnumerable<LotteryOrRedeemCount>> FindAllAsync() =>
            await _context.LotteryOrRedeemLimits.ToListAsync();

        public async Task<LotteryOrRedeemCount> FindByIdAsync(params Guid[] keyValues) =>
            await _context.LotteryOrRedeemLimits.SingleOrDefaultAsync(
                lorl => lorl.ActivityId.Value == keyValues[0] && lorl.UserId.Value == keyValues[1]
            );

        public void Remove(LotteryOrRedeemCount item) =>
            _context.LotteryOrRedeemLimits.Remove(item);

        public void Update(LotteryOrRedeemCount item) =>
            _context.LotteryOrRedeemLimits.Update(item);
    }
}
