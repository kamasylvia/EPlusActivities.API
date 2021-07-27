using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class ActivityUserRepository : RepositoryBase<ActivityUser>
    {
        public ActivityUserRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params Guid[] keyValues) =>
            await _context.ActivityUserLinks.AnyAsync(
                lorl => lorl.ActivityId.Value == keyValues[0] && lorl.UserId == keyValues[1]
            );

        public override async Task<ActivityUser> FindByIdAsync(params Guid[] keyValues) =>
            await _context.ActivityUserLinks.SingleOrDefaultAsync(
                lorl => lorl.ActivityId.Value == keyValues[0] && lorl.UserId.Value == keyValues[1]
            );
    }
}
