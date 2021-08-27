using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileService.Data.Repositories
{
    public class AppFileRepository : RepositoryBase<AppFile>, IAppFileRepository
    {
        public AppFileRepository(ApplicationDbContext context) : base(context) { }

        public async Task<AppFile> FindByAlternateKeyAsync(Guid ownerId, string key)
        {
            return await _context.Files.SingleOrDefaultAsync(
                file => file.OwnerId.Value == ownerId && file.Key == key
            );
        }

        public async Task<IEnumerable<AppFile>> FindByOwnerIdAsync(Guid ownerId) =>
            await _context.Files.Where(file => file.OwnerId.Value == ownerId).ToListAsync();
    }
}
