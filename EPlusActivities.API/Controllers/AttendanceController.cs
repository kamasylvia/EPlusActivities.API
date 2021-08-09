using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.AttendanceDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class AttendanceController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ActivityUser> _activityUserRepository;
        private readonly ILogger<AttendanceController> _logger;
        private readonly IActivityRepository _activityRepository;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly IMemberService _memberService;

        public AttendanceController(
            IAttendanceRepository attendanceRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IActivityRepository activityRepository,
            IIdGeneratorService idGeneratorService,
            IRepository<ActivityUser> activityUserRepository,
            ILogger<AttendanceController> logger,
            IMemberService memberService
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
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
        ) {
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
        public async Task<ActionResult<AttendanceDto>> GetByIdAsync(
            [FromBody] AttendanceForGetByIdDto attendanceDto
        ) {
            var attendance = await _attendanceRepository.FindByIdAsync(attendanceDto.Id.Value);
            return attendance is null
                ? BadRequest("Could not find the attendance.")
                : Ok(_mapper.Map<AttendanceDto>(attendance));
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<AttendanceDto>> AttendAsync(
            [FromBody] AttendanceForAttendDto attendanceDto
        ) {
            #region Parameter validation
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

            var today = DateTime.Now.Date;
            var attendanceDays = activityUser.AttendanceDays ?? 0;
            var sequentialAttendanceDays = activityUser.SequentialAttendanceDays ?? 0;

            if (activityUser.LastAttendanceDate == today)
            {
                return Conflict("Duplicate attendance.");
            }

            #region Update LastAttendanceDate and SequentialAttendanceDays
            activityUser.SequentialAttendanceDays = IsSequential(
                activityUser.LastAttendanceDate,
                today
            ) ? sequentialAttendanceDays + 1 : 1;
            activityUser.LastAttendanceDate = today;
            activityUser.AttendanceDays = ++attendanceDays;
            #endregion

            #region Update credits
            var memberForUpdateCreditRequestDto = new MemberForUpdateCreditRequestDto
            {
                memberId = user.MemberId,
                points = attendanceDto.EarnedCredits,
                reason = attendanceDto.Reason,
                sheetId = _idGeneratorService.NextId().ToString(),
                updateType = CreditUpdateType.Addition
            };

            var (memberUpdateSucceed, memberForUpdateCreditResponseDto) =
                await _memberService.UpdateCreditAsync(
                attendanceDto.UserId.Value,
                memberForUpdateCreditRequestDto
            );
            if (!memberUpdateSucceed)
            {
                _logger.LogError("Failed to update the member.");
                return new InternalServerErrorObjectResult("Failed to update member.");
            }

            activityUser.User.Credit += attendanceDto.EarnedCredits;
            if (activityUser.User.Credit != memberForUpdateCreditResponseDto.Body.Content.NewPoints)
            {
                _logger.LogError("Local credits did not equal to the member's new points.");
                return new InternalServerErrorObjectResult(
                    "Local credits did not equal to the member's new points."
                );
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
            attendance.User = user;
            attendance.Activity = activity;
            attendance.Date = DateTime.Now.Date;
            #endregion

            #region Database operations
            _activityUserRepository.Update(activityUser);
            await _attendanceRepository.AddAsync(attendance);
            var succeeded = await _attendanceRepository.SaveAsync();
            #endregion

            if (!succeeded)
            {
                var error = "Update database exception.";
                _logger.LogError(error);
                return new InternalServerErrorObjectResult(error);
            }

            return Ok(_mapper.Map<AttendanceDto>(attendance));
        }

        private bool IsSequential(DateTime? dateTime1, DateTime dateTime2) =>
            dateTime1.HasValue ? dateTime1.Value.AddDays(1).Date == dateTime2.Date : false;
    }
}
