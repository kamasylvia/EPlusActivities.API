using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    [CustomDependency(ServiceLifetime.Scoped)]
    public class CategoryRepository : RepositoryBase<Category>, INameExistsRepository<Category>
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.Categories
                .AnyAsync(c => c.Id == (Guid)keyValues.FirstOrDefault());

        public async Task<bool> ExistsAsync(string name) =>
            await _context.Categories.AnyAsync(c => c.Name == name);

        public async Task<IEnumerable<Category>> FindByContainedNameAsync(string name) =>
            await _context.Categories
                .Where(c => c.Name.Contains(name))
                .ToListAsync();

        public async Task<Category> FindByNameAsync(string name) =>
            await _context.Categories
                .Where(c => c.Name == name)
                .SingleOrDefaultAsync();
    }
}
