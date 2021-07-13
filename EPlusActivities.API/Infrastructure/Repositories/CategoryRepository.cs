using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class CategoryRepository : RepositoryBase, IRepository<Category>
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(Category item) => await _context.Categories.AddAsync(item);

        public async Task<bool> ExistsAsync(Guid id) => 
            await _context.Categories.AnyAsync(c => c.Id == id);

        public async Task<IEnumerable<Category>> FindAllAsync() => 
            await _context.Categories.ToArrayAsync();

        public async Task<Category> FindByIdAsync(params object[] keyValues) => 
            await _context.Categories.FindAsync(keyValues);

        public void Remove(Category item) => _context.Categories.Remove(item);

        public void Update(Category item) => _context.Categories.Update(item);
    }
}