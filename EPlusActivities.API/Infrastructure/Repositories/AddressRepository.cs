using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class AddressRepository : RepositoryBase<Address>, IFindByParentIdRepository<Address>
    {
        public AddressRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Address>> FindByParentIdAsync(Guid userId) =>
            await _context.Addresses.AsAsyncEnumerable()
                .Where(a => a.UserId == userId)
                .ToListAsync();

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.Addresses.AsAsyncEnumerable()
                .AnyAsync(a => a.Id == (Guid)keyValues.FirstOrDefault());
    }
}
