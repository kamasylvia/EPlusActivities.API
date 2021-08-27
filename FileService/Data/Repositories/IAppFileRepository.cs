using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileService.Entities;

namespace FileService.Data.Repositories
{
    public interface IAppFileRepository : IRepository<AppFile>
    {
        Task<IEnumerable<AppFile>> FindByOwnerIdAsync(Guid ownerId);
        Task<AppFile> FindByAlternateKeyAsync(Guid ownerId, string key);
    }
}
