using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.AttendanceDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.AttendanceQueries
{
    public class GetAttendanceQuery : IRequest<AttendanceDto>
    {
        /// <summary>
        /// 签到 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }
    }
}
