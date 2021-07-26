using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs.AttendanceDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        private readonly IActivityRepository _activityRepository;

        public AttendanceController(
            AttendanceRepository attendanceRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IActivityRepository activityRepository,
            IRepository<ActivityUser> activityUserRepository
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _attendanceRepository =
                attendanceRepository
                ?? throw new ArgumentNullException(nameof(attendanceRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet("user")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetByUserIdAsync(
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
                : Ok(attendance);
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> AttendAsync([FromBody] AttendanceDto attendanceDto)
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

            if (!await _activityRepository.ExistsAsync(attendanceDto.ActivityId.Value))
            {
                return BadRequest("Could not find the activity.");
            }
            #endregion

            var activityUser = await _activityUserRepository.FindByIdAsync(
                attendanceDto.ActivityId.Value,
                attendanceDto.UserId.Value
            );
            #region Update the user's LastAttendanceDate and SequentialAttendanceDays
            var attended = AttendHelper(activityUser);
            if (!attended)
            {
                return Conflict("Duplicate attendance.");
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return new InternalServerErrorObjectResult(updateResult.Errors);
            }
            #endregion

            #region New an entity
            var attendance = _mapper.Map<Attendance>(attendanceDto);
            attendance.Date = DateTime.Now.Date;
            #endregion

            #region Database operations
            await _attendanceRepository.AddAsync(attendance);
            var succeeded = await _attendanceRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<AttendanceDto>(attendance))
                : new InternalServerErrorObjectResult("Update database exception.");
        }

        private bool IsSequential(DateTime? dateTime1, DateTime dateTime2) =>
            dateTime1.HasValue ? dateTime1.Value.AddDays(1).Date == dateTime2.Date : false;

        private bool AttendHelper(ActivityUser activityUser)
        {
            var now = DateTime.Now.Date;
            var attendanceDays = activityUser.AttendanceDays ?? 0;
            var sequentialAttendanceDays = activityUser.SequentialAttendanceDays ?? 0;

            if (activityUser.LastAttendanceDate == now)
            {
                return false;
            }

            #region Update the user's LastAttendanceDate and SequentialAttendanceDays
            activityUser.SequentialAttendanceDays = IsSequential(
                activityUser.LastAttendanceDate,
                now
            ) ? sequentialAttendanceDays + 1 : 1;
            activityUser.LastAttendanceDate = now;
            activityUser.AttendanceDays = ++attendanceDays;
            #endregion

            #region Update credit
            activityUser.User.Credit += sequentialAttendanceDays < 7
                ? sequentialAttendanceDays * 10
                : 70;
            #endregion

            return true;
        }
    }
}
