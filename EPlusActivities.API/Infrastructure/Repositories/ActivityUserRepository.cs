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
    [Repository(ServiceLifetime.Scoped)]
    public class ActivityUserRepository
        : RepositoryBase<ActivityUser>,
          IFindByParentIdRepository<ActivityUser>
    {
        public ActivityUserRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.ActivityUserLinks
                .AsAsyncQueryable()
                .AnyAsync(
                    activityUser =>
                        activityUser.ActivityId.Value == (Guid)keyValues[0]
                        && activityUser.UserId == (Guid)keyValues[1]
                );

        public override async Task<ActivityUser> FindByIdAsync(params object[] keyValues) =>
            await _context.ActivityUserLinks
                .AsAsyncQueryable()
                .SingleOrDefaultAsync(
                    activityUser =>
                        activityUser.ActivityId.Value == (Guid)keyValues[0]
                        && activityUser.UserId.Value == (Guid)keyValues[1]
                );

        public async Task<IEnumerable<ActivityUser>> FindByParentIdAsync(Guid userId) =>
            await _context.ActivityUserLinks
                .AsAsyncQueryable()
                .Where(activityUser => activityUser.UserId == userId)
                .ToListAsync();
    }
}
