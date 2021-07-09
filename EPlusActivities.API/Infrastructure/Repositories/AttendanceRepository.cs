using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class AttendanceRepository : RepositoryBase, IRepository<Attendance>
    {
        public AttendanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task AddAsync(Attendance item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Attendance>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Attendance> FindByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Remove(Attendance item)
        {
            throw new NotImplementedException();
        }

        public void Update(Attendance item)
        {
            throw new NotImplementedException();
        }
    }
}