using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Services.ActivityService
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetAllAvailableActivitiesAsync(
            DateTime startTime,
            DateTime? endTime = null
        );
        Task<IEnumerable<ActivityUser>> BindUserWithActivities(
            Guid userId,
            IEnumerable<Guid> activityIds
        );
        Task<IEnumerable<ActivityUser>> BindUserWithAllAvailableActivities(Guid userId);
    }
}
