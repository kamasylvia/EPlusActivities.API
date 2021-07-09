using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class LotteryRepository : RepositoryBase, IUserIdRepository<Lottery>
    {
        public LotteryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(Lottery item) =>
            await _context.Lotteries.AddAsync(item);

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Lotteries.AnyAsync(wr => wr.Id == id);

        public async Task<IEnumerable<Lottery>> FindAllAsync() =>
            await _context.Lotteries.ToListAsync();

        public async Task<Lottery> FindByIdAsync(Guid id) =>
            await _context.Lotteries.FindAsync(id);

        public async Task<IEnumerable<Lottery>> FindByUserIdAsync(Guid userId) =>
            await _context.Lotteries.Where(a => a.WinnerId == userId).ToListAsync();

        public void Remove(Lottery item) => _context.Lotteries.Remove(item);

        public void Update(Lottery item) => _context.Lotteries.Update(item);
    }
}