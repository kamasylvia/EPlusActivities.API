using Elf.WebAPI.Attributes;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    [AutomaticDependencyInjection(ServiceLifetime.Scoped)]
    public class CouponRepository : RepositoryBase<Coupon>
    {
        public CouponRepository(ApplicationDbContext context) : base(context) { }
    }
}
