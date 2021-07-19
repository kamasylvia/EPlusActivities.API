using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class BrandRepository : RepositoryBase, INameExistsRepository<Brand>
    {
        public BrandRepository(ApplicationDbContext context) :
            base(context)
        {
        }

        public async Task AddAsync(Brand item) =>
            await _context.Brands.AddAsync(item);

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Brands.AnyAsync(b => b.Id == id);

        public async Task<bool> ExistsAsync(string name) =>
            await _context.Brands.AnyAsync(b => b.Name == name);

        public async Task<IEnumerable<Brand>> FindAllAsync() =>
            await _context.Brands.ToListAsync();

        public async Task<IEnumerable<Brand>>
        FindByContainedNameAsync(string name) =>
            await _context
                .Brands
                .Where(p => p.Name.Contains(name))
                .ToListAsync();

        public async Task<Brand> FindByIdAsync(Guid id) =>
            await _context.Brands.FindAsync(id);

        public async Task<Brand> FindByNameAsync(string name) =>
            await _context
                .Brands
                .Where(p => p.Name == name)
                .SingleOrDefaultAsync();

        public void Remove(Brand item) => _context.Brands.Remove(item);

        public void Update(Brand item) => _context.Brands.Update(item);
    }
}
