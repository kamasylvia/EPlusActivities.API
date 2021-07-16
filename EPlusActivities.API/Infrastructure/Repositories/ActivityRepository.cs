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
    public class ActivityRepository : RepositoryBase, IActivityRepository
    {
        public ActivityRepository(ApplicationDbContext context) :
            base(context)
        {
        }

        public async Task AddAsync(Activity item) =>
            await _context.Activities.AddAsync(item);

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Activities.AnyAsync(a => a.Id == id);

        public async Task<IEnumerable<Activity>> FindAllAsync() =>
            await _context.Activities.ToListAsync();

        public async Task<IEnumerable<Activity>>
        FindAllAvailableAsync(DateTime date) =>
            await _context
                .Activities
                .Where(a =>
                    a.StartTime <= date &&
                    (!a.EndTime.HasValue || date <= a.EndTime.Value))
                .ToListAsync();

        public async Task<Activity> FindByIdAsync(Guid id) =>
            await _context.Activities.FindAsync(id);

        public void Remove(Activity item) => _context.Activities.Remove(item);

        public void Update(Activity item) => _context.Activities.Update(item);
    }
}
