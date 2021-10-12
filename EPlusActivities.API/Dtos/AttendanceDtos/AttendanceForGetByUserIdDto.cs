using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.AttendanceDtos
{
    public class AttendanceForGetByUserIdDto
    {
        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        /// <summary>
        /// 起始时间
        /// </summary>
        /// <value></value>
        [Required]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束时间，null 表示至今
        /// </summary>
        /// <value></value>
        public DateTime? EndDate { get; set; }
    }
}
