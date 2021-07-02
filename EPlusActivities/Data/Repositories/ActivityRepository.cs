using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EPlusActivities.API.Data.Repositories
{
    public class ActivityRepository : RepositoryBase, IRepository<Activity>
    {
        public ActivityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(Activity item) => await _context.Activities.AddAsync(item);

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Activities.AnyAsync(a => a.Id == id);

        public async Task<IEnumerable<Activity>> FindAllAsync() =>
            await _context.Activities.ToListAsync();

        public async Task<Activity> FindByIdAsync(Guid id) =>
            await _context.Activities.FindAsync(id);

        public void Remove(Activity item) => _context.Activities.Remove(item);

        public void Update(Activity item) => _context.Activities.Update(item);
    }
}