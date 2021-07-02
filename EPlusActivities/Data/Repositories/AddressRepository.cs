using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EPlusActivities.API.Data.Repositories
{
    public class AddressRepository : RepositoryBase, IAddressRepository
    {
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(Address address) =>
            await _context.Addresses.AddAsync(address);

        public async Task<Address> FindByIdAsync(Guid id) =>
            id.ToString() is null ? null : await _context.Addresses.FindAsync(id);

        public async Task<IEnumerable<Address>> FindAllAsync() =>
            await _context.Addresses.ToListAsync();

        public void Remove(Address address) => _context.Addresses.Remove(address);

        public void Update(Address address) => _context.Addresses.Update(address);

        public async Task<IEnumerable<Address>> FindByUserIdAsync(Guid userId) =>
            await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
    }
}