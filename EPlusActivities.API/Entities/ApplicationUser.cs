using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using EPlusActivities.API.Infrastructure.Enums;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        // 注册渠道
        public ChannelCode RegisterChannel { get; set; }

        // 登录渠道
        public ChannelCode LoginChannel { get; set; }

        // 注册日期
        [Required]
        public DateTime RegisterDate { get; set; }

        // 上次登陆日期
        public DateTime? LastLoginDate { get; set; }

        // 积分
        public int Credit { get; set; }

        public bool IsMember { get; set; }

        public string MemberId { get; set; }

        // 外键
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }

        public virtual ICollection<Attendance> AttendanceRecord { get; set; }

        public virtual ICollection<Lottery> LotteryResults { get; set; }

        public virtual ICollection<ActivityUser> ActivityUserLinks { get; set; }

        public override string ToString() =>
            JsonSerializer
                .Serialize(this,
                new JsonSerializerOptions { WriteIndented = true });
    }
}
