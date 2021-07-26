using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class ActivityUserRepository : RepositoryBase, IRepository<ActivityUser>
    {
        public ActivityUserRepository(ApplicationDbContext context) : base(context) { }

        public async Task AddAsync(ActivityUser item) =>
            await _context.ActivityUserLinks.AddAsync(item);

        public async Task<bool> ExistsAsync(params Guid[] keyValues) =>
            await _context.ActivityUserLinks.AnyAsync(
                lorl => lorl.ActivityId.Value == keyValues[0] && lorl.UserId == keyValues[1]
            );

        public async Task<IEnumerable<ActivityUser>> FindAllAsync() =>
            await _context.ActivityUserLinks.ToListAsync();

        public async Task<ActivityUser> FindByIdAsync(params Guid[] keyValues) =>
            await _context.ActivityUserLinks.SingleOrDefaultAsync(
                lorl => lorl.ActivityId.Value == keyValues[0] && lorl.UserId.Value == keyValues[1]
            );

        public void Remove(ActivityUser item) => _context.ActivityUserLinks.Remove(item);

        public void Update(ActivityUser item) => _context.ActivityUserLinks.Update(item);
    }
}
