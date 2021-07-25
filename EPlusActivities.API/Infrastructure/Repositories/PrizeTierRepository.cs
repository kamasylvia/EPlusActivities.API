using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class PrizeTierRepository : RepositoryBase, IFindByParentIdRepository<PrizeTier>
    {
        public PrizeTierRepository(ApplicationDbContext context) : base(context) { }

        public async Task AddAsync(PrizeTier item) => await _context.PrizeTiers.AddAsync(item);

        public async Task<bool> ExistsAsync(params Guid[] keyValues) =>
            await _context.PrizeTiers.AnyAsync(pt => pt.Id == keyValues.FirstOrDefault());

        public async Task<IEnumerable<PrizeTier>> FindAllAsync() =>
            await _context.PrizeTiers.ToListAsync();

        public async Task<PrizeTier> FindByIdAsync(params Guid[] keyValues) =>
            await _context.PrizeTiers.Include(pt => pt.Activity)
                .SingleOrDefaultAsync(pt => pt.Id == keyValues.FirstOrDefault());

        public async Task<IEnumerable<PrizeTier>> FindByParentIdAsync(Guid userId) =>
            await _context.PrizeTiers.Where(a => a.Activity.Id == userId).ToListAsync();

        public void Remove(PrizeTier item) => _context.Remove(item);

        public void Update(PrizeTier item) => _context.Update(item);
    }
}
