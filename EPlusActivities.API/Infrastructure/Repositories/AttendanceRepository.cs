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

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.AttendanceRecord.AnyAsync(a => a.Id == id);

        public async Task<IEnumerable<Attendance>> FindAllAsync() =>
            await _context.AttendanceRecord.ToListAsync();

        public async Task<Attendance> FindByIdAsync(Guid id) =>
            await _context.AttendanceRecord.FindAsync(id);

        public async Task<IEnumerable<Attendance>> FindByUserIdAsync(
            Guid userId,
            DateTime startTime
        ) =>
            await _context.AttendanceRecord.Where(
                    a => a.UserId == userId && a.Date >= startTime.Date
                )
                .ToListAsync();

        public void Remove(Attendance item) => _context.AttendanceRecord.Remove(item);

        public void Update(Attendance item) => _context.AttendanceRecord.Update(item);
    }
}
