using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EPlusActivities.API.Data.Repositories
{
    public class PrizeRepository : RepositoryBase, IRepository<Prize>
    {
        public PrizeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(Prize prize) => await _context.Prizes.AddAsync(prize);

        public void Remove(Prize prize) => _context.Prizes.Remove(prize);

        public void Update(Prize prize) => _context.Prizes.Update(prize);

        public async Task<IEnumerable<Prize>> FindAllAsync() =>
            await _context.Prizes.ToListAsync();

        public async Task<Prize> FindByIdAsync(Guid id) =>
            await _context.Prizes.FindAsync(id);

        public async Task<IEnumerable<Prize>> FindPrizesByUserIdAsync(Guid userId) =>
            await _context.WinningResults.Where(wr => wr.WinnerId == userId)
                                         .Select(wr => wr.PrizeItem)
                                         .ToListAsync();

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Prizes.AnyAsync(p => p.Id == id);
    }
}