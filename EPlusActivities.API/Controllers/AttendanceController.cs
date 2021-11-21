using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.AttendanceCommands;
using EPlusActivities.API.Application.Queries.AttendanceQueries;
using EPlusActivities.API.Dtos.AttendanceDtos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 签到 API
    /// </summary>
    [ApiController]
    [Route("choujiang/api/[controller]")]
    public class AttendanceController : Controller
    {
        private readonly IMediator _mediator;

        public AttendanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 获取指定用户某个时间段内的签到记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("user")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetByUserIdAsync(
            [FromQuery] GetAttendanceRecordsQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 获取单个签到记录的信息
        /// </summary>
        /// <param name="attendanceDto"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<AttendanceDto>> GetByIdAsync(
            [FromQuery] GetAttendanceQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "customer, tester"
        )]
        public async Task<ActionResult<AttendanceDto>> AttendAsync(
            [FromBody] AttendCommand request
        ) => Ok(await _mediator.Send(request));
    }
}
