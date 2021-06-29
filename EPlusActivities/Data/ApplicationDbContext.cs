using System;
using System.Collections.Generic;
using System.Text.Json;
using EPlusActivities.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,
        ApplicationRole,
        string,
        IdentityUserClaim<string>,
        ApplicationUserRole,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>
    {
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<Prize> Prizes { get; set; }
        public virtual DbSet<WinningResult> WinningResults { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // many-to-many
            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();

                userRole.HasOne(ur => ur.User)
                        .WithMany(u => u.UserRoles)
                        .HasForeignKey(ur => ur.UserId)
                        .IsRequired();
            });

            // one-to-many
            builder.Entity<ApplicationUser>()
                   .HasMany(u => u.Addresses)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId);
            builder.Entity<ApplicationUser>()
                   .HasMany(u => u.WinningResults)
                   .WithOne(a => a.Winner)
                   .HasForeignKey(a => a.WinnerId);

            // one-to-one
            builder.Entity<WinningResult>()
                   .HasOne(result => result.Activity)
                   .WithOne(activity => activity.WinningResult)
                   .HasForeignKey<Activity>(activity => activity.WinningResultId);
            builder.Entity<WinningResult>()
                   .HasOne(result => result.PrizeItem)
                   .WithOne(prize => prize.WinningResult)
                   .HasForeignKey<Prize>(prize => prize.WinningResultId);


            /*
            Seed data
            */
            var seedUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Seed",
                NormalizedUserName = "Seed".ToUpper(),
                PhoneNumber = "11111111111"
            };
            var seedRole = new ApplicationRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Seed",
                NormalizedName = "Seed".ToUpper()
            };

            builder.Entity<ApplicationUser>().HasData(seedUser);
            builder.Entity<ApplicationRole>().HasData(seedRole);
            builder.Entity<ApplicationUserRole>().HasData(
                    new ApplicationUserRole
                    {
                        UserId = seedUser.Id,
                        RoleId = seedRole.Id,
                    }
                );

            // Seed roles
            var roleData = System.IO.File.ReadAllText("Data/RoleSeedData.json");
            var roles = JsonSerializer.Deserialize<List<ApplicationRole>>(roleData);
            roles.ForEach(role =>
                {
                    role.Id = Guid.NewGuid().ToString();
                    role.NormalizedName = role.Name.ToUpper();
                    builder.Entity<ApplicationRole>().HasData(role);
                });

            // Seed others
            var addressId = Guid.NewGuid().ToString();
            var prizeId = Guid.NewGuid().ToString();
            var activityId = Guid.NewGuid().ToString();
            var resultId = Guid.NewGuid().ToString();
            builder.Entity<Prize>().HasData(
                new Prize
                {
                    Id = prizeId,
                    Name = "Seed",
                    WinningResultId = resultId
                }
            );
            builder.Entity<WinningResult>().HasData(
                new WinningResult
                {
                    Id = resultId,
                    WinnerId = seedUser.Id,
                    ActivityId = activityId,
                    PrizeId = prizeId,
                }
            );
            builder.Entity<Activity>().HasData(
                new Activity
                {
                    Id = activityId,
                    Name = "Seed",
                }
            );
            builder.Entity<Address>().HasData(
                new Address
                {
                    Id = addressId,
                    UserId = seedUser.Id,
                }
            );

        }
    }
}