using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs.AttendanceDtos;
using EPlusActivities.API.DTOs.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Yitter.IdGenerator;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class AttendanceController : Controller
    {
        private readonly IMapper _mapper;
        private readonly AttendanceRepository _attendanceRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ActivityUser> _activityUserRepository;
        private readonly ILogger<AttendanceController> _logger;
        private readonly IActivityRepository _activityRepository;
        private readonly IMemberService _memberService;

        public AttendanceController(
            AttendanceRepository attendanceRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IActivityRepository activityRepository,
            IRepository<ActivityUser> activityUserRepository,
            ILogger<AttendanceController> logger,
            IMemberService memberService
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
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

        [HttpGet("user")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<AttendanceForAttendDto>>> GetByUserIdAsync(
            [FromBody] AttendanceForGetByUserIdDto attendanceDto
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(attendanceDto.UserId.ToString());
            if (user is null)
            {
                return BadRequest("Could not find the user.");
            }

            if (!await _activityRepository.ExistsAsync(attendanceDto.ActivityId.Value))
            {
                return BadRequest("Could not find the activity.");
            }
            #endregion

            var attendanceRecord = await _attendanceRepository.FindByUserIdAsync(
                userId: attendanceDto.UserId.Value,
                activityId: attendanceDto.ActivityId.Value,
                startDate: attendanceDto.StartDate.Value,
                endDate: attendanceDto.EndDate
            );

            return attendanceRecord.Count() > 0
                ? Ok(attendanceRecord)
                : NotFound("Could not find any attendances.");
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<AttendanceForAttendDto>> GetByIdAsync(
            [FromBody] AttendanceForGetByIdDto attendanceDto
        )
        {
            var attendance = await _attendanceRepository.FindByIdAsync(attendanceDto.Id.Value);
            return attendance is null
                ? BadRequest("Could not find the attendance.")
                : Ok(attendance);
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<AttendanceDto>> AttendAsync([FromBody] AttendanceForAttendDto attendanceDto)
        {
            #region Parameter validation
            if (await _attendanceRepository.ExistsAsync(attendanceDto.Id.Value))
            {
                return Conflict("This attendance is already existed.");
            }

            var user = await _userManager.FindByIdAsync(attendanceDto.UserId.ToString());
            if (user is null)
            {
                return BadRequest("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(attendanceDto.ActivityId.Value);
            if (activity is null)
            {
                return BadRequest("Could not find the activity.");
            }
            #endregion

            #region Update user and member
            var activityUser = await _activityUserRepository.FindByIdAsync(
                attendanceDto.ActivityId.Value,
                attendanceDto.UserId.Value
            );

            var now = DateTime.Now.Date;
            var attendanceDays = activityUser.AttendanceDays ?? 0;
            var sequentialAttendanceDays = activityUser.SequentialAttendanceDays ?? 0;

            if (activityUser.LastAttendanceDate == now)
            {
                return Conflict("Duplicate attendance.");
            }

            #region Update LastAttendanceDate and SequentialAttendanceDays
            activityUser.SequentialAttendanceDays = IsSequential(
                activityUser.LastAttendanceDate,
                now
            ) ? sequentialAttendanceDays + 1 : 1;
            activityUser.LastAttendanceDate = now;
            activityUser.AttendanceDays = ++attendanceDays;
            #endregion

            #region Update credits
            var memberForUpdateCreditRequestDto = new MemberForUpdateCreditRequestDto
            {
                memberId = user.MemberId,
                points = attendanceDto.EarnedCredits,
                reason = attendanceDto.Reason,
                sheetId = YitIdHelper.NextId().ToString(),
                updateType = CreditUpdateType.Addition
            };

            var (memberUpdateSucceed, memberForUpdateCreditResponseDto) =
                 await _memberService.UpdateCreditAsync(memberForUpdateCreditRequestDto);
            if (!memberUpdateSucceed)
            {
                _logger.LogError("Failed to update the member.");
                return new InternalServerErrorObjectResult("Failed to update member.");
            }

            activityUser.User.Credit += attendanceDto.EarnedCredits;
            if (activityUser.User.Credit != memberForUpdateCreditResponseDto.Body.Content.NewPoints)
            {
                _logger.LogError("Local credits did not equal to the member's new points.");
                return new InternalServerErrorObjectResult("Local credits did not equal to the member's new points.");
            }
            #endregion

            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                _logger.LogError("Failed to update the user.");
                return new InternalServerErrorObjectResult(updateUserResult.Errors);
            }
            #endregion

            #region New an entity
            var attendance = _mapper.Map<Attendance>(attendanceDto);
            attendance.Date = DateTime.Now.Date;
            #endregion

            #region Database operations
            await _activityUserRepository.AddAsync(activityUser);
            await _attendanceRepository.AddAsync(attendance);
            var succeeded = await _attendanceRepository.SaveAsync();
            #endregion

            if (succeeded)
            {
                return Ok(_mapper.Map<AttendanceForAttendDto>(attendance));
            }

            var error = "Update database exception.";
            _logger.LogError(error);
            return new InternalServerErrorObjectResult(error);
        }

        private bool IsSequential(DateTime? dateTime1, DateTime dateTime2) =>
            dateTime1.HasValue ? dateTime1.Value.AddDays(1).Date == dateTime2.Date : false;
    }
}
