using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class PrizeTierRepository
        : RepositoryBase<PrizeTier>,
          IFindByParentIdRepository<PrizeTier>
    {
        public PrizeTierRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.PrizeTiers.AnyAsync(pt => pt.Id == (Guid)keyValues.FirstOrDefault());

        public override async Task<PrizeTier> FindByIdAsync(params object[] keyValues) =>
            await _context.PrizeTiers.Include(pt => pt.Activity)
                .SingleOrDefaultAsync(pt => pt.Id == (Guid)keyValues.FirstOrDefault());

        public async Task<IEnumerable<PrizeTier>> FindByParentIdAsync(Guid userId) =>
            await _context.PrizeTiers.Where(a => a.Activity.Id == userId).ToListAsync();
    }
}
