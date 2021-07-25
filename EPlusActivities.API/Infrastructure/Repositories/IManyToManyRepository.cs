using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IManyToManyRepository<T> where T : class
    {
        Task AddAsync(T item);

        void Remove(T item);

        void Update(T item);

        Task<T> FindByIdAsync(Guid id1, Guid id2);

        Task<bool> SaveAsync();
    }
}
