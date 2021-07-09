using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IPrizeRepository : IRepository<Prize>
    {
         Task<IEnumerable<Prize>> FindByNameAsync(string name);
    }
}