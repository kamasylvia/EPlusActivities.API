using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elf.WebAPI.Attributes;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    [AutomaticDependencyInjection(ServiceLifetime.Scoped)]
    public class ActivityRepository : RepositoryBase<Activity>, IActivityRepository
    {
        public ActivityRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.Activities.AnyAsync(a => a.Id == (Guid)keyValues.FirstOrDefault());

        public async Task<IEnumerable<Activity>> FindAvailableActivitiesAsync(DateTime date) =>
            await _context.Activities
                .AsAsyncEnumerable()
                .Where(a => !(a.StartTime > date) && !(date > a.EndTime))
                .ToListAsync();

        public async Task<IEnumerable<Activity>> FindActivitiesAsync(
            DateTime startTime,
            DateTime? endTime
        ) =>
            await _context.Activities
                .AsAsyncEnumerable()
                .Where(a => startTime <= a.StartTime && !(a.EndTime > endTime))
                .ToListAsync();

        public override async Task<Activity> FindByIdAsync(params object[] keyValues) =>
            await _context.Activities
                .Include(a => a.LotteryDetailStatement)
                .Include(a => a.PrizeTiers)
                .SingleOrDefaultAsync(a => a.Id == (Guid)keyValues.FirstOrDefault());

        public async Task<Activity> FindByActivityCodeAsync(string activityCode) =>
            await _context.Activities
                .Include(a => a.LotteryDetailStatement)
                .SingleOrDefaultAsync(a => a.ActivityCode == activityCode);

        public async Task<Activity> FindWithActivityUserLink(Guid id) =>
            await _context.Activities
                .Include(a => a.ActivityUserLinks)
                .SingleOrDefaultAsync(a => a.Id == id);

        public async Task<Activity> FindWithPrizeType(Guid id) =>
            await _context.Activities
                .Include(a => a.PrizeTiers)
                .SingleOrDefaultAsync(a => a.Id == id);
    }
}
