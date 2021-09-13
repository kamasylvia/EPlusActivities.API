using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class PrizeItemRepository : RepositoryBase<PrizeItem>, IPrizeItemRepository
    {
        public PrizeItemRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<PrizeItem> FindByIdAsync(params object[] keyValues) =>
            await _context.PrizeItems.Include(prizeItem => prizeItem.Brand)
                .Include(prizeItem => prizeItem.Category)
                .SingleOrDefaultAsync(
                    prizeItem => prizeItem.Id == (Guid)keyValues.FirstOrDefault()
                );

        public async Task<IEnumerable<PrizeItem>> FindByNameAsync(string name) =>
            await _context.PrizeItems.AsAsyncEnumerable()
                .Where(p => p.Name.Contains(name))
                .ToArrayAsync();

        public async Task<IEnumerable<PrizeItem>> FindByPrizeTierIdAsync(Guid id) =>
            await _context.PrizeItems.Include(pi => pi.PrizeTierPrizeItems)
                .ThenInclude(ptpi => ptpi.PrizeTier)
                .Where(pt => pt.PrizeTierPrizeItems.Any(ptpi => ptpi.PrizeTier.Id == id))
                .ToListAsync();

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.PrizeItems.AsAsyncEnumerable()
                .AnyAsync(p => p.Id == (Guid)keyValues.FirstOrDefault());
    }
}
