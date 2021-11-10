using System;
using System.IO;
using System.Linq;
using FileService.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileService.Data
{
    public class DbInitializer
    {
        public static void CreateStorageDirectory(IApplicationBuilder app)
        {
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            Directory.CreateDirectory(configuration["FileStorageDirectory"]);
        }

        public static void Initialize(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            using (
                var serviceScope = app.ApplicationServices
                    .GetService<IServiceScopeFactory>()
                    .CreateScope()
            )
            {
                var context =
                    serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                context.Database.Migrate();

                // Look for any Data.
                if (context.Files.Any())
                {
                    return; // DB has been seeded
                }

                SeedData(context);
            }
        }

        private static void SeedData(ApplicationDbContext context)
        {
            var seedPhoto = new AppFile
            {
                OwnerId = Guid.NewGuid(),
                Key = "Seed",
                ContentType = "application/json"
            };
            context.Files.Add(seedPhoto);
            context.SaveChanges();
        }
    }
}
