using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Services.IdentityServer;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Lottery = EPlusActivities.API.Entities.Lottery;

namespace EPlusActivities.API.Data
{
    public class DbInitializer
    {
        private static List<ApplicationRole> _roles =
            JsonSerializer.Deserialize<List<ApplicationRole>>(
                System.IO.File.ReadAllText("Data/RoleSeedData.json")
            );
        private static List<ApplicationUser> _users =
            JsonSerializer.Deserialize<List<ApplicationUser>>(
                System.IO.File.ReadAllText("Data/UserSeedData.json")
            );

        public static void Initialize(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            using (
                var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()
                    .CreateScope()
            ) {
                var context =
                    serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager =
                    serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager =
                    serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                if (environment.IsDevelopment())
                {
                    var deleted = context.Database.EnsureDeleted();
                    System.Console.WriteLine($"The old database is deleted: {deleted}");
                    var created = context.Database.EnsureCreated();
                    System.Console.WriteLine($"The new database is created: {created}");
                }

                if (environment.IsProduction())
                {
                    var persistedGrantDbContext =
                        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                    var configurationDbContext =
                        serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                    persistedGrantDbContext.Database.EnsureDeleted();
                    configurationDbContext.Database.EnsureDeleted();

                    persistedGrantDbContext.Database.Migrate();
                    configurationDbContext.Database.Migrate();

                    context.Database.EnsureDeleted();
                    context.Database.Migrate();
                    SeedConfig(configurationDbContext);
                }

                // Look for any Data.
                if (context.Users.Any())
                {
                    return; // DB has been seeded
                }

                SeedRoles(roleManager);
                SeedUsers(userManager);
                SeedData(context, userManager);
            }
        }

        private static void SeedConfig(ConfigurationDbContext configurationDbContext)
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

            configurationDbContext.SaveChanges();
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager) =>
            _users.ForEach(
                user =>
                {
                    if (userManager.FindByNameAsync(user.UserName).Result is null)
                    {
                        var result = userManager.CreateAsync(user).Result;
                        result =
                            userManager.AddToRoleAsync(
                                user,
                                _roles.SingleOrDefault(r => r.Name.ToLower() == "seed").Name
                            ).Result;
                    }
                }
            );

        private static void SeedRoles(RoleManager<ApplicationRole> roleManager) =>
            _roles.ForEach(
                role =>
                {
                    if (!roleManager.RoleExistsAsync(role.Name).Result)
                    {
                        var result = roleManager.CreateAsync(role).Result;
                    }
                }
            );

        private static void SeedData(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
        ) {
            var user = userManager.FindByNameAsync("seed").Result;

            #region Seed Addresses
            var address = new Address { User = user };
            context.Addresses.Add(address);
            #endregion

            #region Seed AttendanceRecord
            var attendance = new Attendance { User = user, Date = DateTime.MinValue };
            context.AttendanceRecord.Add(attendance);
            #endregion

            #region Seed Activities
            var activity = new Activity("Seed")
            {
                StartTime = DateTime.MinValue,
                EndTime = DateTime.MinValue,
                ActivityCode = "a00000000000"
            };
            context.Activities.Add(activity);
            #endregion

            #region Seed Brands
            var brand = new Brand("Seed");
            context.Brands.Add(brand);
            #endregion

            #region Seed Categories
            var category = new Category("Seed");
            context.Categories.Add(category);
            #endregion

            #region Seed PrizeItems
            var prizeItem = new PrizeItem("Seed") { Brand = brand, Category = category, };
            context.PrizeItems.Add(prizeItem);
            #endregion

            #region Seed PrizeTiers
            var prizeTier = new PrizeTier("Seed") { Activity = activity };
            context.PrizeTiers.Add(prizeTier);
            #endregion

            #region Seed PrizeTierPrizeItem
            var prizeTypePrizeItem = new PrizeTierPrizeItem
            {
                PrizeItem = prizeItem,
                PrizeTier = prizeTier
            };
            context.PrizeTierPrizeItems.Add(prizeTypePrizeItem);
            #endregion

            #region Seed LotteryResults
            var lottery = new Lottery
            {
                User = user,
                Activity = activity,
                Date = DateTime.MinValue
            };
            context.LotteryResults.Add(lottery);
            #endregion

            #region Seed ActivityUser
            var activityUser = new ActivityUser { Activity = activity, User = user, };
            context.ActivityUserLinks.Add(activityUser);
            #endregion

            #region Seed Coupons
            var coupon = new Coupon { User = user, PrizeItem = prizeItem, Code = "Seed" };
            context.Coupons.Add(coupon);
            #endregion

            #region Seed Administrator
            var admin = new ApplicationUser { UserName = "admin" };
            var tester = new ApplicationUser { UserName = "tester" };
            var result = userManager.CreateAsync(admin, "Pa$$w0rd").Result;
            result =
                userManager.AddToRoleAsync(
                    admin,
                    _roles.SingleOrDefault(r => r.Name.ToLower() == "admin").Name
                ).Result;
            result = userManager.CreateAsync(tester, "Pa$$w0rd").Result;
            result =
                userManager.AddToRoleAsync(
                    tester,
                    _roles.SingleOrDefault(r => r.Name.ToLower() == "tester").Name
                ).Result;
            #endregion

            context.SaveChanges();
        }
    }
}
