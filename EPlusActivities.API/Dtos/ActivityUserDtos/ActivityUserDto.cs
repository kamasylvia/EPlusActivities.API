using System;

namespace EPlusActivities.API.Dtos.ActivityUserDtos
{
    public record ActivityUserDto
    {
        /// <summary>
        /// 全活动周期已抽奖次数
        /// </summary>
        /// <value></value>
        public int UsedDraws { get; set; }

        /// <summary>
        /// 当天已抽奖次数
        /// </summary>
        /// <value></value>
        public int TodayUsedDraws { get; set; }

        /// <summary>
        /// 剩余抽奖次数，null 表示无限
        /// </summary>
        /// <value></value>
        public int RemainingDraws { get; set; }

        /// <summary>
        /// 签到天数，非签到活动中用 null 表示
        /// </summary>
        /// <value></value>
        public int? AttendanceDays { get; set; }

        /// <summary>
        /// 连续签到天数，非签到活动中用 null 表示
        /// </summary>
        /// <value></value>
        public int? SequentialAttendanceDays { get; set; }

        /// <summary>
        /// 兑换一次抽奖所需积分，null 表示非抽奖活动
        /// </summary>
        /// <value></value>
        public int? RequiredCreditForRedeeming { get; set; }

        /// <summary>
        /// 上次签到日期
        /// </summary>
        /// <value></value>
        public DateTime? LastAttendanceDate { get; set; }

        public Guid? UserId { get; set; }

        public Guid? ActivityId { get; set; }
    }
}
