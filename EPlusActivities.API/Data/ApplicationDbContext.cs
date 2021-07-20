using System;
using System.Collections.Generic;
using System.Text.Json;
using EPlusActivities.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Data
{
    public class
    ApplicationDbContext
    :
    IdentityDbContext<ApplicationUser,
        ApplicationRole,
        Guid,
        IdentityUserClaim<Guid>,
        ApplicationUserRole,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>
    >
    {
        public virtual DbSet<Address> Addresses { get; set; }

        public virtual DbSet<Attendance> AttendanceRecord { get; set; }

        public virtual DbSet<Activity> Activities { get; set; }

        public virtual DbSet<Brand> Brands { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<PrizeItem> PrizeItems { get; set; }

        public virtual DbSet<PrizeType> PrizeTypes { get; set; }

        public virtual DbSet<PrizeTypePrizeItem> PrizeTypePrizeItems
        { get; set;
        }

        public virtual DbSet<Lottery> LotteryResults { get; set; }

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options
        ) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

#region Set unique properties
            builder
                .Entity<ApplicationUser>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            builder.Entity<Brand>().HasIndex(b => b.Name).IsUnique();
            builder.Entity<Category>().HasIndex(b => b.Name).IsUnique();
#endregion

            /*
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
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId)
                   .IsRequired();
            builder.Entity<ApplicationUser>()
                   .HasMany(u => u.AttendanceRecord)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId)
                   .IsRequired();
            #endregion

            #region one-to-one
            builder.Entity<PrizeItem>()
                   .HasOne(prizeItem => prizeItem.Lottery)
                   .WithOne(lottery => lottery.PrizeItemItem)
                   .HasForeignKey<Lottery>(lottery => lottery.PrizeItemId)
                   .IsRequired();
            builder.Entity<Activity>()
                   .HasOne(activity => activity.Lottery)
                   .WithOne(lottery => lottery.ActivityItem)
                   .HasForeignKey<Lottery>(lottery => lottery.ActivityId)
                   .IsRequired();
            #endregion

            #endregion
            */
        }
    }
}
