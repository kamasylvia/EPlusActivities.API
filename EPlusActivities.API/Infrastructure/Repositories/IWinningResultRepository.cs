using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IWinningResultRepository : IRepository<WinningResult>
    {
        Task<IEnumerable<WinningResult>> FindByUserIdAsync(string userId);
    }
}