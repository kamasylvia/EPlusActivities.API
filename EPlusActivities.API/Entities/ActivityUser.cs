using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class ActivityUser
    {
        // 剩余抽奖次数，null 表示无限
        public int? RemainingDraws { get; set; }

        // 当天剩余抽奖次数
        public int? TodayRemainingDraws { get; set; }

        // 签到天数
        public int? AttendanceDays { get; set; }

        // 连续签到天数
        public int? SequentialAttendanceDays { get; set; }

        // 上次签到日期
        public DateTime? LastAttendanceDate { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        public Activity Activity { get; set; }
    }
}
