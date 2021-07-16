using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IActivityRepository : IRepository<Activity>
    {
        public Task<IEnumerable<Activity>> FindAllAvailableAsync(DateTime date);
    }
}
