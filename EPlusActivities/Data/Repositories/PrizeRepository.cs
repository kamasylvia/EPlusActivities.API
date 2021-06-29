using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.Data.Repositories
{
    public class PrizeRepository : RepositoryBase, IRepository<Prize>
    {
        public PrizeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(Prize prize) => await _context.Prizes.AddAsync(prize);

        public void Remove(Prize prize) => _context.Prizes.Remove(prize);

        public void Update(Prize prize) => _context.Prizes.Update(prize);

        public async Task<IEnumerable<Prize>> GetAllAsync() =>
            await _context.Prizes.ToListAsync();

        public async Task<Prize> GetByIdAsync(string id) =>
            await _context.Prizes.FindAsync(id);

        public async Task<IEnumerable<Prize>> GetPrizesByUserIdAsync(string userId) =>
            await _context.WinningResults.Where(wr => wr.WinnerId == userId)
                                         .Select(wr => wr.PrizeItem)
                                         .ToListAsync();
    }
}