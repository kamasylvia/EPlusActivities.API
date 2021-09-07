using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.AttendanceDtos
{
    public class AttendanceForAttendDto
    {
        /// <summary>
        /// 用户访问的渠道
        /// </summary>
        /// <value></value>
        public string ChannelCode { get; set; }

        /// <summary>
        /// 签到获得积分
        /// </summary>
        /// <value></value>
        public int EarnedCredits { get; set; }

        /// <summary>
        /// 更新积分原因
        /// </summary>
        /// <value></value>
        public string Reason { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }
    }
}
