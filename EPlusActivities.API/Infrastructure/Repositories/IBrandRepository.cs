using System.Threading.Tasks;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IBrandRepository : IRepository<Brand>
    {
        Task<bool> ExistsAsync(string name);
        Task<Brand> FindByNameAsync(string name);
    }
}