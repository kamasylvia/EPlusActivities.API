using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileService.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T item);

        void Remove(T item);

        void Update(T item);

        Task<IEnumerable<T>> FindAllAsync();

        Task<T> FindByIdAsync(params object[] keyValues);

        Task<bool> SaveAsync();
    }
}
