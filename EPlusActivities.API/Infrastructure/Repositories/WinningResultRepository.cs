using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class WinningResultRepository : RepositoryBase, IWinningResultRepository
    {
        public WinningResultRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(WinningResult item) =>
            await _context.WinningResults.AddAsync(item);

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.WinningResults.AnyAsync(wr => wr.Id == id);

        public async Task<IEnumerable<WinningResult>> FindAllAsync() =>
            await _context.WinningResults.ToListAsync();

        public async Task<WinningResult> FindByIdAsync(Guid id) =>
            await _context.WinningResults.FindAsync(id);

        public async Task<IEnumerable<WinningResult>> FindByUserIdAsync(Guid userId) =>
            await _context.WinningResults.Where(a => a.WinnerId == userId).ToListAsync();

        public void Remove(WinningResult item) => _context.WinningResults.Remove(item);

        public void Update(WinningResult item) => _context.WinningResults.Update(item);
    }
}