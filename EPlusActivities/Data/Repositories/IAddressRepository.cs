using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.Entities;

namespace EPlusActivities.Data.Repositories
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<IEnumerable<Address>> GetByUserIdAsync(string userId);
    }
}