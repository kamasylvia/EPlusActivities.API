using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class CategoryRepository : RepositoryBase, INameExistsRepository<Category>
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(Category item) => await _context.Categories.AddAsync(item);

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Categories.AnyAsync(c => c.Id == id);

        public async Task<bool> ExistsAsync(string name) =>
            await _context.Categories.AnyAsync(c => c.Name == name);

        public async Task<IEnumerable<Category>> FindAllAsync() =>
            await _context.Categories.ToArrayAsync();

        public async Task<Category> FindByIdAsync(Guid id) =>
            await _context.Categories.FindAsync(id);

        public async Task<Category> FindByNameAsync(string name) =>
            await _context.Categories.Where(c => c.Name == name)
                                     .SingleOrDefaultAsync();

        public void Remove(Category item) => _context.Categories.Remove(item);

        public void Update(Category item) => _context.Categories.Update(item);
    }
}