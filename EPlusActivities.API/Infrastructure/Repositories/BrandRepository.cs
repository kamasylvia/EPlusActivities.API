using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elf.WebAPI.Attributes;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    [AutomaticDependencyInjection(ServiceLifetime.Scoped)]
    public class BrandRepository : RepositoryBase<Brand>, INameExistsRepository<Brand>
    {
        public BrandRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.Brands.AnyAsync(b => b.Id == (Guid)keyValues.FirstOrDefault());

        public async Task<bool> ExistsAsync(string name) =>
            await _context.Brands.AnyAsync(b => b.Name == name);

        public async Task<IEnumerable<Brand>> FindByContainedNameAsync(string name) =>
            await _context.Brands
                .AsAsyncEnumerable()
                .Where(p => p.Name.Contains(name))
                .ToListAsync();

        public async Task<Brand> FindByNameAsync(string name) =>
            await _context.Brands.Where(p => p.Name == name).SingleOrDefaultAsync();
    }
}
