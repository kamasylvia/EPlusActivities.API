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
    public class ActivityRepository : RepositoryBase<Activity>, IActivityRepository
    {
        public ActivityRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params Guid[] keyValues) =>
            await _context.Activities.AnyAsync(a => a.Id == keyValues.FirstOrDefault());

        public async Task<IEnumerable<Activity>> FindAvailableActivitiesAsync(DateTime date) =>
            await _context.Activities.Where(
                    a => a.StartTime <= date && (!a.EndTime.HasValue || date <= a.EndTime.Value)
                )
                .ToListAsync();

        public override async Task<Activity> FindByIdAsync(params Guid[] keyValues) =>
            await _context.Activities.Include(a => a.LotteryResults)
                .Include(a => a.PrizeTiers)
                .SingleOrDefaultAsync(a => a.Id == keyValues.FirstOrDefault());

        public async Task<Activity> FindWithActivityUserLink(Guid id) =>
            await _context.Activities.Include(a => a.ActivityUserLinks)
                .SingleOrDefaultAsync(a => a.Id == id);

        public async Task<Activity> FindWithPrizeType(Guid id) =>
            await _context.Activities.Include(a => a.PrizeTiers)
                .SingleOrDefaultAsync(a => a.Id == id);
    }
}
