using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    [CustomDependency(ServiceLifetime.Scoped)]
    public class ActivityUserRepository : RepositoryBase<ActivityUser>, IActivityUserRepository
    {
        public ActivityUserRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.ActivityUserLinks
                .AnyAsync(
                    activityUser =>
                        activityUser.ActivityId.Value == (Guid)keyValues[0]
                        && activityUser.UserId == (Guid)keyValues[1]
                );

        public override async Task<ActivityUser> FindByIdAsync(params object[] keyValues) =>
            await _context.ActivityUserLinks
                .SingleOrDefaultAsync(
                    activityUser =>
                        activityUser.ActivityId.Value == (Guid)keyValues[0]
                        && activityUser.UserId.Value == (Guid)keyValues[1]
                );

        public async Task<IEnumerable<ActivityUser>> FindByUserIdAsync(Guid userId) =>
            await _context.ActivityUserLinks
                .Include(au => au.Activity)
                .AsAsyncEnumerable()
                .Where(au => au.UserId == userId)
                .ToListAsync();

        public async Task<IEnumerable<ActivityUser>> FindByActivityIdAsync(Guid activityId) =>
            await _context.ActivityUserLinks
                .Where(au => au.ActivityId == activityId)
                .ToListAsync();
    }
}
