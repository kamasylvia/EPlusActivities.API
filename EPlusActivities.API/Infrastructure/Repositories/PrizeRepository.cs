using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class PrizeRepository : RepositoryBase, IFindByNameRepository<Prize>
    {
        public PrizeRepository(ApplicationDbContext context) :
            base(context)
        {
        }

        public async Task AddAsync(Prize prize) =>
            await _context.Prizes.AddAsync(prize);

        public void Remove(Prize prize) => _context.Prizes.Remove(prize);

        public void Update(Prize prize) => _context.Prizes.Update(prize);

        public async Task<IEnumerable<Prize>> FindAllAsync() =>
            await _context.Prizes.ToListAsync();

        public async Task<Prize> FindByIdAsync(Guid id) =>
            await _context
                .Prizes
                .Include(prize => prize.Brand)
                .Include(prize => prize.Category)
                .SingleOrDefaultAsync(prize => prize.Id == id);

        public async Task<IEnumerable<Prize>> FindByNameAsync(string name) =>
            await _context
                .Prizes
                .Where(p => p.Name.Contains(name))
                .ToArrayAsync();

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Prizes.AnyAsync(p => p.Id == id);
    }
}
