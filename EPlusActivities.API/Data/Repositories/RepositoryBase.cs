using System.Threading.Tasks;

namespace EPlusActivities.API.Data.Repositories
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