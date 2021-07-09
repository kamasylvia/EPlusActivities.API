using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class LotteryResultRepository : RepositoryBase, ILotteryResultRepository
    {
        public LotteryResultRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(LotteryResult item) =>
            await _context.LotteryResults.AddAsync(item);

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.LotteryResults.AnyAsync(wr => wr.Id == id);

        public async Task<IEnumerable<LotteryResult>> FindAllAsync() =>
            await _context.LotteryResults.ToListAsync();

        public async Task<LotteryResult> FindByIdAsync(Guid id) =>
            await _context.LotteryResults.FindAsync(id);

        public async Task<IEnumerable<LotteryResult>> FindByUserIdAsync(Guid userId) =>
            await _context.LotteryResults.Where(a => a.WinnerId == userId).ToListAsync();

        public void Remove(LotteryResult item) => _context.LotteryResults.Remove(item);

        public void Update(LotteryResult item) => _context.LotteryResults.Update(item);
    }
}