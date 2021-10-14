using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface ILotteryRepository : IRepository<Lottery>
    {
        Task<IEnumerable<Lottery>> FindByUserIdAsync(Guid userId);

        Task<IEnumerable<Lottery>> FindByActivityIdAsync(Guid activityId);
    }
}
