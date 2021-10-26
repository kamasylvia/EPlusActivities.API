using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.AttendanceDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.AttendanceCommands
{
    public class GetAttendanceRecordsCommand : IRequest<IEnumerable<AttendanceDto>>
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
