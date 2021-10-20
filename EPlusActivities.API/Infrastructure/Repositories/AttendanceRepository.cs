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
    public class AttendanceRepository : RepositoryBase<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.AttendanceRecord
                .AsAsyncQueryable()
                .AnyAsync(a => a.Id == (Guid)keyValues.FirstOrDefault());

        public async Task<IEnumerable<Attendance>> FindByUserIdAsync(
            Guid userId,
            Guid activityId,
            DateTime startDate,
            DateTime? endDate
        ) =>
            await _context.AttendanceRecord
                .AsAsyncQueryable()
                .Where(
                    a =>
                        a.User.Id == userId
                        && a.Activity.Id == activityId
                        && a.Date >= startDate.Date
                        && !(a.Date > endDate)
                )
                .ToListAsync();
    }
}
