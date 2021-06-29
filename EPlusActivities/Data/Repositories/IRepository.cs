using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPlusActivities.Data.Repositories
{
    public interface IRepository<T>
    {
        Task AddAsync(T item);
        void Remove(T item);
        void Update(T item);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
    }
}