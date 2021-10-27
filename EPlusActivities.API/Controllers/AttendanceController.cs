using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Application.Commands.AttendanceCommands;
using EPlusActivities.API.Dtos.AttendanceDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        private readonly IMapper _mapper;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFindByParentIdRepository<ActivityUser> _activityUserRepository;
        private readonly ILogger<AttendanceController> _logger;
        private readonly IActivityRepository _activityRepository;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly IMemberService _memberService;
        private readonly IMediator _mediator;

        public AttendanceController(
            IAttendanceRepository attendanceRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IActivityRepository activityRepository,
            IIdGeneratorService idGeneratorService,
            IFindByParentIdRepository<ActivityUser> activityUserRepository,
            ILogger<AttendanceController> logger,
            IMemberService memberService,
            IMediator mediator
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _mediator = mediator;
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _attendanceRepository =
                attendanceRepository
                ?? throw new ArgumentNullException(nameof(attendanceRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
            [FromQuery] GetAttendanceRecordsCommand request
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
            [FromQuery] AttendanceForGetByIdDto attendanceDto
        )
        {
            var attendance = await _attendanceRepository.FindByIdAsync(attendanceDto.Id.Value);
            return attendance is null
              ? BadRequest("Could not find the attendance.")
              : Ok(_mapper.Map<AttendanceDto>(attendance));
        }

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
