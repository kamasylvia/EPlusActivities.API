using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class ActivityUserRepository
        : RepositoryBase<ActivityUser>,
          IFindByParentIdRepository<ActivityUser>
    {
        public ActivityUserRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params Guid[] keyValues) =>
            await _context.ActivityUserLinks.AnyAsync(
                activityUser =>
                    activityUser.ActivityId.Value == keyValues[0]
                    && activityUser.UserId == keyValues[1]
            );

        public override async Task<ActivityUser> FindByIdAsync(params Guid[] keyValues) =>
            await _context.ActivityUserLinks.SingleOrDefaultAsync(
                activityUser =>
                    activityUser.ActivityId.Value == keyValues[0]
                    && activityUser.UserId.Value == keyValues[1]
            );

        public async Task<IEnumerable<ActivityUser>> FindByParentIdAsync(Guid userId) =>
            await _context.ActivityUserLinks.Where(activityUser => activityUser.UserId == userId)
                .ToListAsync();
    }
}
