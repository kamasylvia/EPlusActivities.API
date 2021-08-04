using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class CreditRepository : RepositoryBase<Credit>
    {
        public CreditRepository(ApplicationDbContext context) : base(context) { }
    }
}
