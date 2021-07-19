using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using EPlusActivities.API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace EPlusActivities.API.Data
{
    public class DbInitializer
    {
        private static List<ApplicationRole> _roles = JsonSerializer.Deserialize<List<ApplicationRole>>(System.IO.File.ReadAllText("Data/RoleSeedData.json"));
        private static List<ApplicationUser> _users = JsonSerializer.Deserialize<List<ApplicationUser>>(System.IO.File.ReadAllText("Data/UserSeedData.json"));

        public static void Initialize(
            IWebHostEnvironment environment,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            if (environment.IsDevelopment())
            {
                var deleted = context.Database.EnsureDeleted();
                System.Console.WriteLine($"The old database is deleted: {deleted}");
                var created = context.Database.EnsureCreated();
                System.Console.WriteLine($"The new database is created: {created}");
            }

            if (environment.IsProduction())
            {
                context.Database.Migrate();
            }

            // Look for any Data.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedData(context, userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager) =>
            _users.ForEach(
                user =>
                {
                    if (userManager.FindByNameAsync(user.UserName).Result is null)
                    {
                        var result = userManager.CreateAsync(user).Result;
                        result = userManager.AddToRoleAsync(user, _roles.SingleOrDefault(r =>
                            r.Name.ToLower() == "seed").Name).Result;
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
            UserManager<ApplicationUser> userManager)
        {
            var user = userManager.FindByNameAsync("seed").Result;

            #region Seed Addresses
            var address = new Address { User = user };
            context.Addresses.Add(address);
            #endregion

            #region Seed AttendanceRecord
            var attendance = new Attendance { User = user };
            context.AttendanceRecord.Add(attendance);
            #endregion

            #region Seed Activities
            var activity = new Activity("Seed")
            {
                EndTime = DateTime.MinValue
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
            var prizeItem = new PrizeItem("Seed")
            {
                Brand = brand,
                Category = category,
            };
            context.PrizeItems.Add(prizeItem);
            #endregion

            #region Seed PrizeTypes
            var prizeType = new PrizeType("Seed")
            {
                Activity = activity
            };
            context.PrizeTypes.Add(prizeType);
            #endregion

            #region Seed LotteryResults
            var lottery = new Lottery
            {
                User = user,
                PrizeItem = prizeItem,
                Activity = activity,
                Channel = "Seed Data"
            };
            context.LotteryResults.Add(lottery);
            #endregion

            context.SaveChanges();
        }
    }
}