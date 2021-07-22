using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IPrizeItemRepository : IRepository<PrizeItem>
    {
        Task<IEnumerable<PrizeItem>> FindByNameAsync(string name);

        Task<IEnumerable<PrizeItem>> FindByPrizeTypeIdAsync(Guid id);
    }
}
