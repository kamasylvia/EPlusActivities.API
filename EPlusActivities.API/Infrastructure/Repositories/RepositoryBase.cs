using System.Threading.Tasks;
using EPlusActivities.API.Data;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public abstract class RepositoryBase
    {
        protected readonly ApplicationDbContext _context;
        public RepositoryBase(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> SaveAsync() => await _context.SaveChangesAsync() >= 0;

    }
}