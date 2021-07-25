using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class LotteryRepository : RepositoryBase, IFindByParentIdRepository<Lottery>
    {
        public LotteryRepository(ApplicationDbContext context) : base(context) { }

        public async Task AddAsync(Lottery item) => await _context.LotteryResults.AddAsync(item);

        public async Task<bool> ExistsAsync(params Guid[] keyValues) =>
            await _context.LotteryResults.AnyAsync(lr => lr.Id == keyValues.FirstOrDefault());

        public async Task<IEnumerable<Lottery>> FindAllAsync() =>
            await _context.LotteryResults.ToListAsync();

        public async Task<Lottery> FindByIdAsync(params Guid[] keyValues) =>
            await _context.LotteryResults.Include(lottery => lottery.Activity)
                .Include(lottery => lottery.PrizeItem)
                .SingleOrDefaultAsync(lottery => lottery.Id == keyValues.FirstOrDefault());

        public async Task<IEnumerable<Lottery>> FindByParentIdAsync(Guid userId) =>
            await _context.LotteryResults.Where(a => a.User.Id == userId).ToListAsync();

        public void Remove(Lottery item) => _context.LotteryResults.Remove(item);

        public void Update(Lottery item) => _context.LotteryResults.Update(item);
    }
}
