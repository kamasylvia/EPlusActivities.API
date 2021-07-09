using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface ILotteryResultRepository : IRepository<LotteryResult>
    {
        Task<IEnumerable<LotteryResult>> FindByUserIdAsync(Guid userId);
    }
}