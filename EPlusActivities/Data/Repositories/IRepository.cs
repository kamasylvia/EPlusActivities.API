using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EPlusActivities.API.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T item);
        void Remove(T item);
        void Update(T item);
        Task<IEnumerable<T>> FindAllAsync();
        Task<T> FindByIdAsync(Guid id);
        Task<bool> SaveAsync();
        async Task<bool> ExistsAsync(Guid id) => await FindByIdAsync(id) is not null;
    }
}