using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.AttendanceDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.AttendanceCommands
{
    public class GetAttendanceCommand : IRequest<AttendanceDto>
    {
        /// <summary>
        /// 签到 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }
    }
}
