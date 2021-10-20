using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Services.IdentityServer;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EPlusActivities.API.Data
{
    public class ConfigurationDbContextSeeder
    {
        public async Task SeedAsync(IHost host)
        {
            using (
                var serviceScope = host.Services.GetService<IServiceScopeFactory>().CreateScope()
            )
            {
                var environment =
                    serviceScope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                if (environment.IsProduction())
                {
                    var persistedGrantDbContext =
                        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                    var configurationDbContext =
                        serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                    persistedGrantDbContext.Database.Migrate();
                    configurationDbContext.Database.Migrate();

                    await SeedConfigAsync(configurationDbContext);
                }
            }
        }
        private async Task SeedConfigAsync(ConfigurationDbContext configurationDbContext)
        {
            if (!configurationDbContext.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    configurationDbContext.Clients.Add(client.ToEntity());
                }
            }

            if (!configurationDbContext.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    configurationDbContext.IdentityResources.Add(resource.ToEntity());
                }
            }

            if (!configurationDbContext.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources)
                {
                    configurationDbContext.ApiResources.Add(resource.ToEntity());
                }
            }

            if (!configurationDbContext.ApiScopes.Any())
            {
                foreach (var resource in Config.ApiScopes)
                {
                    configurationDbContext.ApiScopes.Add(resource.ToEntity());
                }
            }

            await configurationDbContext.SaveChangesAsync();
        }
    }
}
