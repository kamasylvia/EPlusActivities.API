using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IFindByParentIdRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> FindByParentIdAsync(Guid userId);
    }
}
