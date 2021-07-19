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
    public class PrizeItemRepository : RepositoryBase, IFindByNameRepository<PrizeItem>
    {
        public PrizeItemRepository(ApplicationDbContext context) :
            base(context)
        {
        }

        public async Task AddAsync(PrizeItem prizeItem) =>
            await _context.PrizeItems.AddAsync(prizeItem);

        public void Remove(PrizeItem prizeItem) => _context.PrizeItems.Remove(prizeItem);

        public void Update(PrizeItem prizeItem) => _context.PrizeItems.Update(prizeItem);

        public async Task<IEnumerable<PrizeItem>> FindAllAsync() =>
            await _context.PrizeItems.ToListAsync();

        public async Task<PrizeItem> FindByIdAsync(Guid id) =>
            await _context
                .PrizeItems
                .Include(prizeItem => prizeItem.Brand)
                .Include(prizeItem => prizeItem.Category)
                .SingleOrDefaultAsync(prizeItem => prizeItem.Id == id);

        public async Task<IEnumerable<PrizeItem>> FindByNameAsync(string name) =>
            await _context
                .PrizeItems
                .Where(p => p.Name.Contains(name))
                .ToArrayAsync();

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.PrizeItems.AnyAsync(p => p.Id == id);
    }
}