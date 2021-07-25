using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface INameExistsRepository<T> : IRepository<T> where T : class
    {
        Task<bool> ExistsAsync(string name);

        Task<T> FindByNameAsync(string name);

        Task<IEnumerable<T>> FindByContainedNameAsync(string name);
    }
}
