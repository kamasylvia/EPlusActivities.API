using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class CouponRepository : RepositoryBase<Coupon>
    {
        public CouponRepository(ApplicationDbContext context) : base(context) { }
    }
}
