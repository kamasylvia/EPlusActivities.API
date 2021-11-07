using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FileService.Data.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public RepositoryBase(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public virtual async Task AddAsync(T item) => await _context.Set<T>().AddAsync(item);

        public virtual async Task<IEnumerable<T>> FindAllAsync() =>
            await _context.Set<T>().ToListAsync();

        public virtual async Task<T> FindByIdAsync(params object[] keyValues) =>
            await _context.Set<T>().FindAsync(keyValues);

        public void Remove(T item) => _context.Set<T>().Remove(item);

        public async Task<bool> SaveAsync() => await _context.SaveChangesAsync() >= 0;

        public virtual void Update(T item) => _context.Set<T>().Update(item);
    }
}
