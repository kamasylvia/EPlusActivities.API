using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<
              ApplicationUser,
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

        public virtual DbSet<PrizeTier> PrizeTiers { get; set; }

        public virtual DbSet<ActivityUser> ActivityUserLinks { get; set; }

        public virtual DbSet<PrizeTierPrizeItem> PrizeTierPrizeItems { get; set; }

        public virtual DbSet<Lottery> LotteryResults { get; set; }

        public virtual DbSet<Credit> Credits { get; set; }

        public virtual DbSet<Coupon> Coupons { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            #region Set unique properties
            builder.Entity<ApplicationUser>().HasIndex(u => u.PhoneNumber).IsUnique();
            builder.Entity<Activity>().HasIndex(a => a.ActivityCode).IsUnique();

            // builder.Entity<Activity>().HasAlternateKey(a => a.ActivityCode);
            builder.Entity<Brand>().HasIndex(b => b.Name).IsUnique();
            builder.Entity<Category>().HasIndex(b => b.Name).IsUnique();

            builder.Entity<PrizeTierPrizeItem>()
                .HasKey(ptpi => new { ptpi.PrizeTierId, ptpi.PrizeItemId });

            builder.Entity<ActivityUser>().HasKey(lad => new { lad.ActivityId, lad.UserId });

            builder.Entity<Credit>().HasAlternateKey(c => c.SheetId);
            #endregion



            #region Set list of enum values
            builder.Entity<Activity>()
                .Property(a => a.AvailableChannels)
                .HasConversion(
                    v => string.Join(';', v.Select(e => e.ToString("D")).ToArray()),
                    v =>
                        v.Split(new[] { ';' }, StringSplitOptions.TrimEntries)
                            .Select(e => Enum.Parse(typeof(ChannelCode), e, true))
                            .Cast<ChannelCode>()
                            .ToList()
                );
            #endregion
        }
    }
}
