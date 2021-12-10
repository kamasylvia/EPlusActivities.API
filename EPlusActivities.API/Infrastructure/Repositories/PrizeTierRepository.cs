using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    [CustomDependency(ServiceLifetime.Scoped)]
    public class PrizeTierRepository
        : RepositoryBase<PrizeTier>,
          IFindByParentIdRepository<PrizeTier>
    {
        public PrizeTierRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.PrizeTiers
                .AnyAsync(pt => pt.Id == (Guid)keyValues.FirstOrDefault());

        public override async Task<PrizeTier> FindByIdAsync(params object[] keyValues) =>
            await _context.PrizeTiers
                .Include(pt => pt.Activity)
                .Include(pt => pt.PrizeTierPrizeItems)
                .ThenInclude(ptpi => ptpi.PrizeItem)
                .SingleOrDefaultAsync(pt => pt.Id == (Guid)keyValues.FirstOrDefault());

        public async Task<IEnumerable<PrizeTier>> FindByParentIdAsync(Guid userId) =>
            await _context.PrizeTiers
                .Include(pt => pt.PrizeTierPrizeItems)
                .ThenInclude(ptpi => ptpi.PrizeItem)
                .Where(a => a.Activity.Id == userId)
                .ToListAsync();
    }
}
