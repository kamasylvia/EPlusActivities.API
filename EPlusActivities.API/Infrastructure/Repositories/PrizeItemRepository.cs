using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elf.WebAPI.Attributes;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    [AutomaticDependencyInjection(ServiceLifetime.Scoped)]
    public class PrizeItemRepository : RepositoryBase<PrizeItem>, IPrizeItemRepository
    {
        public PrizeItemRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<PrizeItem> FindByIdAsync(params object[] keyValues) =>
            await _context.PrizeItems
                .Include(prizeItem => prizeItem.Brand)
                .Include(prizeItem => prizeItem.Category)
                .SingleOrDefaultAsync(
                    prizeItem => prizeItem.Id == (Guid)keyValues.FirstOrDefault()
                );

        public async Task<IEnumerable<PrizeItem>> FindByNameAsync(string name) =>
            await _context.PrizeItems.Where(p => p.Name.Contains(name)).ToArrayAsync();

        public async Task<IEnumerable<PrizeItem>> FindByPrizeTierIdAsync(Guid id) =>
            await _context.PrizeItems
                .Include(pi => pi.PrizeTierPrizeItems)
                .ThenInclude(ptpi => ptpi.PrizeTier).AsAsyncEnumerable()
                .Where(pt => pt.PrizeTierPrizeItems.Any(ptpi => ptpi.PrizeTier.Id == id))
                .ToListAsync();

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.PrizeItems.AnyAsync(p => p.Id == (Guid)keyValues.FirstOrDefault());
    }
}
