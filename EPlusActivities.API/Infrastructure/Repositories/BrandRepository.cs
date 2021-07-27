using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class BrandRepository : RepositoryBase<Brand>, INameExistsRepository<Brand>
    {
        public BrandRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params Guid[] keyValues) =>
            await _context.Brands.AnyAsync(b => b.Id == keyValues.FirstOrDefault());

        public async Task<bool> ExistsAsync(string name) =>
            await _context.Brands.AnyAsync(b => b.Name == name);

        public async Task<IEnumerable<Brand>> FindByContainedNameAsync(string name) =>
            await _context.Brands.Where(p => p.Name.Contains(name)).ToListAsync();

        public async Task<Brand> FindByNameAsync(string name) =>
            await _context.Brands.Where(p => p.Name == name).SingleOrDefaultAsync();
    }
}
