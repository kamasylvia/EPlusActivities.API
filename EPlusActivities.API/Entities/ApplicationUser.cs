using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;


namespace EPlusActivities.API.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        // 注册渠道
        public string RegisterChannel { get; set; }

        // 登录渠道
        public string LoginChannel { get; set; }

        // 注册日期
        [Required]
        public DateTime RegisterDate { get; set; }

        // 上次登录日期
        public DateTime? LastActiveDate { get; set; }


        // 积分
        public int Credit { get; set; }

        // 是否会员
        public bool IsMember { get; set; }

        // 签到天数
        public int AttendanceDays { get; set; }

        // 连续签到天数
        public int SequentialAttendanceDays { get; set; }


        // 外键
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<WinningResult> WinningResults { get; set; } 



        public override string ToString() =>
            JsonSerializer.Serialize(
                this,
                new JsonSerializerOptions { WriteIndented = true });
    }
}