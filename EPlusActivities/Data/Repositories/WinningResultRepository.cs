using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.Data.Repositories
{
    public class WinningResultRepository : RepositoryBase, IWinningResultRepository
    {
        public WinningResultRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(WinningResult item) =>
            await _context.WinningResults.AddAsync(item);

        public async Task<IEnumerable<WinningResult>> GetAllAsync() =>
            await _context.WinningResults.ToListAsync();


        public async Task<WinningResult> GetByIdAsync(string id) =>
            await _context.WinningResults.FindAsync(id);

        public async Task<IEnumerable<WinningResult>> GetByUserIdAsync(string userId) =>
            (await _context.Users.FindAsync(userId)).WinningResults;

        public void Remove(WinningResult item) => _context.WinningResults.Remove(item);

        public void Update(WinningResult item) => _context.WinningResults.Update(item);
    }
}