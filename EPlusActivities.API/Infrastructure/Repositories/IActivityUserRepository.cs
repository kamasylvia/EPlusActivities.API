using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IActivityUserRepository : IRepository<ActivityUser>
    {
        Task<IEnumerable<ActivityUser>> FindByUserIdAsync(Guid userId);

        Task<IEnumerable<ActivityUser>> FindByActivityIdAsync(Guid activityId);
    }
}
