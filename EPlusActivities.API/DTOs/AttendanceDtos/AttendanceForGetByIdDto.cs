using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.AttendanceDtos
{
    public class AttendanceForGetByIdDto
    {
        /// <summary>
        /// 签到 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }
    }
}
