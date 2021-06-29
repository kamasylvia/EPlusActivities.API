using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.Entities;

namespace EPlusActivities.Data.Repositories
{
    public interface IWinningResultRepository : IRepository<WinningResult>
    {
        Task<IEnumerable<WinningResult>> GetByUserIdAsync(string userId);
    }
}