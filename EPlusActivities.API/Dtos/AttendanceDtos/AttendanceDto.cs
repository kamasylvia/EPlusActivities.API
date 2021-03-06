using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.AttendanceDtos
{
    public record AttendanceDto
    {
        [Required]
        public Guid? Id { get; set; }

        /// <summary>
        /// 签到日期
        /// </summary>
        /// <value></value>
        [Required]
        public DateOnly? Date { get; set; }

        /// <summary>
        /// 用户访问的渠道，字符串不区分大小写。
        /// 取值范围：Ngs, NgsPlaza, Alldays
        /// </summary>
        /// <value></value>
        public string ChannelCode { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }
    }
}
