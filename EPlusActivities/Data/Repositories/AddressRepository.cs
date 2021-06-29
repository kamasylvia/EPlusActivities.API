using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.Data.Repositories
{
    public class AddressRepository : RepositoryBase, IAddressRepository
    {
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(Address address) =>
            await _context.Addresses.AddAsync(address);

        public async Task<Address> GetByIdAsync(string id) =>
            await _context.Addresses.FindAsync(id);

        public async Task<IEnumerable<Address>> GetAllAsync() =>
            await _context.Addresses.ToListAsync();

        public void Remove(Address address) => _context.Addresses.Remove(address);

        public void Update(Address address) => _context.Addresses.Update(address);

        public async Task<IEnumerable<Address>> GetByUserIdAsync(string userId) =>
            (await _context.Users.FindAsync(userId)).Addresses;
    }
}