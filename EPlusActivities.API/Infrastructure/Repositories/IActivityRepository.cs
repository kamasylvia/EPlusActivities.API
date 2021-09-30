using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IActivityRepository : IRepository<Activity>
    {
        Task<IEnumerable<Activity>> FindActivitiesAsync(DateTime startTime, DateTime? endTime);

        Task<IEnumerable<Activity>> FindAvailableActivitiesAsync(DateTime date);

        Task<Activity> FindWithPrizeType(Guid id);

        Task<Activity> FindWithActivityUserLink(Guid id);
    }
}
