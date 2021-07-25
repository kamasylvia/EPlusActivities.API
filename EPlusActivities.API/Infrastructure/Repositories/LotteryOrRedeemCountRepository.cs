using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class LotteryOrRedeemCountRepository : RepositoryBase, IManyToManyRepository<LotteryOrRedeemCount>
    {
        public LotteryOrRedeemCountRepository(ApplicationDbContext context) : base(context) { }

        public async Task AddAsync(LotteryOrRedeemCount item) =>
            await _context.LotteryOrRedeemLimits.AddAsync(item);

        public async Task<IEnumerable<LotteryOrRedeemCount>> FindAllAsync() =>
            await _context.LotteryOrRedeemLimits.ToListAsync();

        public async Task<LotteryOrRedeemCount> FindByIdAsync(Guid userId, Guid activityId)
        {
            return await _context.LotteryOrRedeemLimits.SingleOrDefaultAsync(
                lorl => lorl.UserId.Value == userId && lorl.ActivityId.Value == activityId
            );
        }

        public void Remove(LotteryOrRedeemCount item) =>
            _context.LotteryOrRedeemLimits.Remove(item);

        public void Update(LotteryOrRedeemCount item) =>
            _context.LotteryOrRedeemLimits.Update(item);
    }
}
