using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class CategoryRepository : RepositoryBase<Category>, INameExistsRepository<Category>
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<bool> ExistsAsync(params object[] keyValues) =>
            await _context.Categories.AsAsyncQueryable()
                .AnyAsync(c => c.Id == (Guid)keyValues.FirstOrDefault());

        public async Task<bool> ExistsAsync(string name) =>
            await _context.Categories.AsAsyncQueryable().AnyAsync(c => c.Name == name);

        public async Task<IEnumerable<Category>> FindByContainedNameAsync(string name) =>
            await _context.Categories.AsAsyncQueryable()
                .Where(c => c.Name.Contains(name))
                .ToListAsync();

        public async Task<Category> FindByNameAsync(string name) =>
            await _context.Categories.AsAsyncQueryable()
                .Where(c => c.Name == name)
                .SingleOrDefaultAsync();
    }
}
