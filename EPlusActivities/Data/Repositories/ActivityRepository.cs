using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.Data.Repositories
{
    public class ActivityRepository : RepositoryBase, IRepository<Activity>
    {
        public ActivityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(Activity item) => await _context.Activities.AddAsync(item);

        public async Task<IEnumerable<Activity>> GetAllAsync() =>
            await _context.Activities.ToListAsync();

        public async Task<Activity> GetByIdAsync(string id) =>
            await _context.Activities.FindAsync(id);

        public void Remove(Activity item) => _context.Activities.Remove(item);

        public void Update(Activity item) => _context.Activities.Update(item);
    }
}