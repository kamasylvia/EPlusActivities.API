using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IFindByNameRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> FindByNameAsync(string name);
    }
}