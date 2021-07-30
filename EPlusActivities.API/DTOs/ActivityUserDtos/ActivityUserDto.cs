using System;

namespace EPlusActivities.API.DTOs.ActivityUserDtos
{
    public class ActivityUserDto
    {
        // 全活动周期剩余抽奖次数
        public int? EntireRemainingDraws { get; set; }

        // 当天剩余抽奖次数
        public int? TodayRemainingDraws { get; set; }

        // 签到天数
        public int? AttendanceDays { get; set; }

        // 连续签到天数
        public int? SequentialAttendanceDays { get; set; }

        // 上次签到日期
        public DateTime? LastAttendanceDate { get; set; }

        public Guid? UserId { get; set; }

        public Guid? ActivityId { get; set; }


    }
}
