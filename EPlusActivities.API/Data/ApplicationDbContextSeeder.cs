using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EPlusActivities.API.Data
{
    public class ApplicationDbContextSeeder
    {
        private static List<ApplicationRole> _roles = JsonSerializer.Deserialize<
            List<ApplicationRole>
        >(System.IO.File.ReadAllText("Data/RoleSeedData.json"));
        private static List<ApplicationUser> _users = JsonSerializer.Deserialize<
            List<ApplicationUser>
        >(System.IO.File.ReadAllText("Data/UserSeedData.json"));

        public async Task SeedAsync(IHost host)
        {
            using (
                var serviceScope = host.Services.GetService<IServiceScopeFactory>().CreateScope()
            )
            {
                var environment =
                    serviceScope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                var configuration =
                    serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
                var context =
                    serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<
                    UserManager<ApplicationUser>
                >();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<
                    RoleManager<ApplicationRole>
                >();

                if (Convert.ToBoolean(configuration["RefreshDbEveryTime"]))
                {
                    var deleted = context.Database.EnsureDeleted();
                    System.Console.WriteLine($"The old database is deleted: {deleted}");
                    var created = context.Database.EnsureCreated();
                    System.Console.WriteLine($"The new database is created: {created}");
                }
                else
                {
                    context.Database.Migrate();
                }

                // Look for any Data.
                if (context.Users.Any())
                {
                    return; // DB has already been seeded.
                }

                await SeedRolesAsync(roleManager);
                await SeedUsersAsync(userManager);
                await SeedDataAsync(context, userManager);
            }
        }

        private async Task SeedUsersAsync(UserManager<ApplicationUser> userManager) =>
            await _users
                .ToAsyncEnumerable()
                .ForEachAwaitAsync(
                    async user =>
                    {
                        if (await userManager.FindByNameAsync(user.UserName) is null)
                        {
                            var result = await userManager.CreateAsync(user);
                            result = await userManager.AddToRoleAsync(
                                user,
                                _roles.SingleOrDefault(r => r.Name.ToLower() == "seed").Name
                            );
                        }
                    }
                );

        private async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager) =>
            await _roles
                .ToAsyncEnumerable()
                .ForEachAwaitAsync(
                    async role =>
                    {
                        if (!await roleManager.RoleExistsAsync(role.Name))
                        {
                            var result = await roleManager.CreateAsync(role);
                        }
                    }
                );

        private async Task SeedDataAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
        )
        {
            var user = await userManager.FindByNameAsync("seed");

            #region Seed Addresses
            var address = new Address { User = user };
            await context.Addresses.AddAsync(address);
            #endregion

            #region Seed AttendanceRecord
            var attendance = new Attendance { User = user, Date = DateTime.MinValue };
            await context.AttendanceRecord.AddAsync(attendance);
            #endregion

            #region Seed Activities
            var activity = new Activity("Seed")
            {
                StartTime = DateTime.MinValue,
                EndTime = DateTime.MinValue,
                ActivityCode = "a00000000000",
                Color = "#000000"
            };
            await context.Activities.AddAsync(activity);
            #endregion

            #region Seed Brands
            var brand = new Brand("Seed");
            await context.Brands.AddAsync(brand);
            #endregion

            #region Seed Categories
            var category = new Category("Seed");
            await context.Categories.AddAsync(category);
            #endregion

            #region Seed PrizeItems
            var prizeItem = new PrizeItem("Seed") { Brand = brand, Category = category, };
            await context.PrizeItems.AddAsync(prizeItem);
            #endregion

            #region Seed PrizeTiers
            var prizeTier = new PrizeTier("Seed") { Activity = activity };
            await context.PrizeTiers.AddAsync(prizeTier);
            #endregion

            #region Seed PrizeTierPrizeItem
            var prizeTypePrizeItem = new PrizeTierPrizeItem
            {
                PrizeItem = prizeItem,
                PrizeTier = prizeTier
            };
            await context.PrizeTierPrizeItems.AddAsync(prizeTypePrizeItem);
            #endregion

            #region Seed LotteryResults
            var lottery = new Lottery
            {
                User = user,
                Activity = activity,
                DateTime = DateTime.MinValue
            };
            await context.LotteryResults.AddAsync(lottery);
            #endregion

            #region Seed ActivityUser
            var activityUser = new ActivityUser { Activity = activity, User = user, };
            await context.ActivityUserLinks.AddAsync(activityUser);
            #endregion

            #region Seed Coupons
            var coupon = new Coupon { User = user, PrizeItem = prizeItem, Code = "Seed" };
            await context.Coupons.AddAsync(coupon);
            #endregion

            #region Seed Statement
            var statement = new GeneralLotteryRecords { Activity = activity };
            await context.GeneralLotteryRecords.AddAsync(statement);
            #endregion

            #region Seed Administrator
            var admin = new ApplicationUser { UserName = "admin" };
            var tester = new ApplicationUser { UserName = "tester" };
            var result = await userManager.CreateAsync(admin, "Pa$$w0rd");
            result = await userManager.AddToRoleAsync(
                admin,
                _roles.SingleOrDefault(r => r.Name.ToLower() == "admin").Name
            );
            result = await userManager.CreateAsync(tester, "Pa$$w0rd");
            result = await userManager.AddToRoleAsync(
                tester,
                _roles.SingleOrDefault(r => r.Name.ToLower() == "tester").Name
            );
            #endregion

            await context.SaveChangesAsync();
        }
    }
}
