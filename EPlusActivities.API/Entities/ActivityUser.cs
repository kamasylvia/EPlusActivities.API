﻿using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Entities
{
    public class ActivityUser
    {
        // 渠道号
        public ChannelCode Channel { get; set; }

        // 全活动周期已抽奖次数
        public int UsedDraws { get; set; }

        // 当天已抽奖次数
        public int TodayUsedDraws { get; set; }

        // 当天已兑换次数
        public int TodayUsedRedempion { get; set; }

        // 剩余抽奖次数
        public int RemainingDraws { get; set; }

        // 签到天数
        public int? AttendanceDays { get; set; }

        // 连续签到天数
        public int? SequentialAttendanceDays { get; set; }

        // 上次签到日期
        public DateTime? LastAttendanceDate { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        public virtual Activity Activity { get; set; }
    }
}
