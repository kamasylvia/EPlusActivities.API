using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Data.Repositories
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<IEnumerable<Address>> FindByUserIdAsync(Guid userId);
    }
}