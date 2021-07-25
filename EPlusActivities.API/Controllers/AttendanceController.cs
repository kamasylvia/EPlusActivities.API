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

        public AttendanceController(
            AttendanceRepository attendanceRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            #endregion

            var attendanceRecord = await _attendanceRepository.FindByUserIdAsync(
                attendanceDto.UserId.Value,
                attendanceDto.Date.Value
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
                return BadRequest("This attendance is already existed.");
            }

            var user = await _userManager.FindByIdAsync(attendanceDto.UserId.ToString());
            if (user is null)
            {
                return BadRequest("Could not find the user.");
            }
            #endregion

            #region Update the user's LastAttendanceDate and SequentialAttendanceDays
            var attended = AttendHelper(user);
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

        private bool AttendHelper(ApplicationUser user)
        {
            var now = DateTime.Now.Date;

            if (user.LastAttendanceDate == now)
            {
                return false;
            }

            #region Update the user's LastAttendanceDate and SequentialAttendanceDays
            user.SequentialAttendanceDays = IsSequential(user.LastAttendanceDate, now)
                ? user.SequentialAttendanceDays + 1
                : 1;
            #endregion

            #region Update credit
            user.Credit += user.SequentialAttendanceDays < 7
                ? user.SequentialAttendanceDays * 10
                : 70;
            #endregion

            user.LastAttendanceDate = now;
            return true;
        }
    }
}
