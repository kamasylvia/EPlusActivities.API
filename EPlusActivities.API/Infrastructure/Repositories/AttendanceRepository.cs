using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class AttendanceRepository : RepositoryBase, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext context) : base(context) { }

        public async Task AddAsync(Attendance item) =>
            await _context.AttendanceRecord.AddAsync(item);

        public async Task<bool> ExistsAsync(params Guid[] keyValues) =>
            await _context.AttendanceRecord.AnyAsync(a => a.Id == keyValues.FirstOrDefault());

        public async Task<IEnumerable<Attendance>> FindAllAsync() =>
            await _context.AttendanceRecord.ToListAsync();

        public async Task<Attendance> FindByIdAsync(params Guid[] keyValues) =>
            await _context.AttendanceRecord.FindAsync(keyValues.FirstOrDefault());

        public async Task<IEnumerable<Attendance>> FindByUserIdAsync(
            Guid userId,
            Guid activityId,
            DateTime startDate,
            DateTime? endDate
        ) =>
            await _context.AttendanceRecord.Where(
                    a =>
                        a.User.Id == userId
                        && a.Activity.Id == activityId
                        && a.Date >= startDate.Date
                        && !(a.Date > endDate)
                )
                .ToListAsync();

        public void Remove(Attendance item) => _context.AttendanceRecord.Remove(item);

        public void Update(Attendance item) => _context.AttendanceRecord.Update(item);
    }
}
