using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class
    PrizeTypeRepository
    : RepositoryBase, INameExistsRepository<PrizeType>
    {
        public PrizeTypeRepository(ApplicationDbContext context) :
            base(context)
        {
        }

        public async Task AddAsync(PrizeType item) =>
            await _context.PrizeTypes.AddAsync(item);

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.PrizeTypes.AnyAsync(pt => pt.Id == id);

        public async Task<bool> ExistsAsync(string name) =>
            await _context.PrizeTypes.AnyAsync(pt => pt.Name == name);

        public async Task<IEnumerable<PrizeType>> FindAllAsync() =>
            await _context.PrizeTypes.ToListAsync();

        public async Task<PrizeType> FindByIdAsync(Guid id) =>
            await _context.PrizeTypes.FindAsync(id);

        public async Task<PrizeType> FindByNameAsync(string name) =>
            await _context
                .PrizeTypes
                .Where(p => p.Name == name)
                .SingleOrDefaultAsync();

        public void Remove(PrizeType item) => _context.Remove(item);

        public void Update(PrizeType item) => _context.Update(item);
    }
}
