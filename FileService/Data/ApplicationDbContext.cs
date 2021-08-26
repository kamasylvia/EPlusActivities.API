using FileService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<AppFile> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppFile>().HasAlternateKey(file => new { file.OwnerId, file.Key });
        }
    }
}
