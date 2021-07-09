using System;
using System.Collections.Generic;
using System.Text.Json;
using EPlusActivities.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,
        ApplicationRole,
        Guid,
        IdentityUserClaim<Guid>,
        ApplicationUserRole,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>
    {
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Attendance> AttendanceRecord { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<Prize> Prizes { get; set; }
        public virtual DbSet<Lottery> Lotteries { get; set; }

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

            #region 构建外键关系
            #region many-to-many
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
            #endregion

            #region one-to-many
            builder.Entity<ApplicationUser>()
                   .HasMany(u => u.Addresses)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId)
                   .IsRequired();
            builder.Entity<ApplicationUser>()
                   .HasMany(u => u.Lotteries)
                   .WithOne(a => a.Winner)
                   .HasForeignKey(a => a.WinnerId)
                   .IsRequired();
            builder.Entity<ApplicationUser>()
                   .HasMany(u => u.AttendanceRecord)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId)
                   .IsRequired();
            #endregion

            #region one-to-one
            builder.Entity<Prize>()
                   .HasOne(prize => prize.Lottery)
                   .WithOne(lottery => lottery.PrizeItem)
                   .HasForeignKey<Lottery>(lottery => lottery.PrizeId)
                   .IsRequired();
            builder.Entity<Activity>()
                   .HasOne(activity => activity.Lottery)
                   .WithOne(lottery => lottery.ActivityItem)
                   .HasForeignKey<Lottery>(lottery => lottery.ActivityId)
                   .IsRequired();
            #endregion

            #endregion

            #region Seed data
            #region Seed users
            var seedUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "Seed",
                NormalizedUserName = "Seed".ToUpper(),
                PhoneNumber = "11111111111"
            };
            var seedRole = new ApplicationRole
            {
                Id = Guid.NewGuid(),
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
            #endregion

            #region Seed roles
            var roleData = System.IO.File.ReadAllText("Data/RoleSeedData.json");
            var roles = JsonSerializer.Deserialize<List<ApplicationRole>>(roleData);
            roles.ForEach(role =>
                {
                    role.Id = Guid.NewGuid();
                    role.NormalizedName = role.Name.ToUpper();
                    builder.Entity<ApplicationRole>().HasData(role);
                });
            #endregion

            #region Seed other data
            var addressId = Guid.NewGuid();
            var prizeId = Guid.NewGuid();
            var activityId = Guid.NewGuid();
            var resultId = Guid.NewGuid();
            var attendanceId = Guid.NewGuid();
            var seedDate = DateTime.Now.Date;
            builder.Entity<Prize>().HasData(
                new Prize
                {
                    Id = prizeId,
                    Name = "Seed",
                    LotteryId = resultId
                }
            );
            builder.Entity<Lottery>().HasData(
                new Lottery
                {
                    Id = resultId,
                    Date = seedDate,
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
                    LotteryId = resultId
                }
            );
            builder.Entity<Address>().HasData(
                new Address
                {
                    Id = addressId,
                    UserId = seedUser.Id,
                }
            );
            builder.Entity<Attendance>().HasData(
                new Attendance
                {
                    Id = attendanceId,
                    Date = seedDate,
                    UserId = seedUser.Id,
                }
            );
            #endregion
            #endregion
        }
    }
}