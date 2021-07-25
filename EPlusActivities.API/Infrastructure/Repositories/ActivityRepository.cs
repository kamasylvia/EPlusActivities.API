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
        public ActivityRepository(ApplicationDbContext context) : base(context) { }

        public async Task AddAsync(Activity item) => await _context.Activities.AddAsync(item);

        public async Task<bool> ExistsAsync(params Guid[] keyValues) =>
            await _context.Activities.AnyAsync(a => a.Id == keyValues.FirstOrDefault());

        public async Task<IEnumerable<Activity>> FindAllAsync() =>
            await _context.Activities.ToListAsync();

        public async Task<IEnumerable<Activity>> FindAllAvailableAsync(DateTime date) =>
            await _context.Activities.Where(
                    a => a.StartTime <= date && (!a.EndTime.HasValue || date <= a.EndTime.Value)
                )
                .ToListAsync();

        public async Task<Activity> FindByIdAsync(params Guid[] keyValues) =>
            await _context.Activities.Include(a => a.LotteryResults)
                .Include(a => a.PrizeTiers)
                .SingleOrDefaultAsync(a => a.Id == keyValues.FirstOrDefault());

        public void Remove(Activity item) => _context.Activities.Remove(item);

        public void Update(Activity item) => _context.Activities.Update(item);
    }
}
