using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Task<IEnumerable<Attendance>> FindByUserIdAsync(Guid userId, DateTime startTime);
    }
}