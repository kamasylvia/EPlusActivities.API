using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    [CustomDependency(ServiceLifetime.Scoped)]
    public class CreditRepository : RepositoryBase<Credit>
    {
        public CreditRepository(ApplicationDbContext context) : base(context) { }
    }
}
