namespace EPlusActivities.Data.Repositories
{
    public abstract class RepositoryBase
    {
        protected readonly ApplicationDbContext _context;
        public RepositoryBase(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}