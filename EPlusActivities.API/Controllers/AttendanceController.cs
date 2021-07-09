using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Utils;
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
            IMapper mapper)
        {
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _attendanceRepository = attendanceRepository
                ?? throw new ArgumentNullException(nameof(attendanceRepository));
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet("list")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetByUserIdAsync([FromBody] AttendanceDto attendanceDto)
        {
            #region 参数验证
            if (!attendanceDto.Date.HasValue)
            {
                return BadRequest("缺少起始日期");
            }

            var user = await _userManager.FindByIdAsync(attendanceDto.UserId.ToString());
            if (user is null)
            {
                return BadRequest("用户不存在");
            }
            #endregion

            var attendanceRecord = await _attendanceRepository.FindByUserIdAsync(attendanceDto.UserId, attendanceDto.Date.Value);
            return Ok(attendanceRecord);
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<AttendanceDto>> GetByIdAsync([FromBody] AttendanceDto attendanceDto)
        {
            var attendance = await _attendanceRepository.FindByIdAsync(attendanceDto.Id);
            return attendance is null ? BadRequest("签到记录不存在") : Ok(attendance);
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> Attend([FromBody] AttendanceDto attendanceDto)
        {
            #region 参数验证
            if (await _attendanceRepository.ExistsAsync(attendanceDto.Id))
            {
                return BadRequest("签到记录已存在");
            }

            var user = await _userManager.FindByIdAsync(attendanceDto.UserId.ToString());
            if (user is null)
            {
                return BadRequest("用户不存在");
            }

            #endregion

            #region 更新用户数据中最近签到日期和连续签到天数
            var attended = AttendanceUtil.AttendHelper(user);
            if (!attended)
            {
                return Conflict("重复签到");
            }
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return new InternalServerErrorObjectResult(updateResult.Errors);
            }
            #endregion

            #region 新建签到记录
            var attendance = _mapper.Map<Attendance>(attendanceDto);
            attendance.Id = Guid.NewGuid();
            attendance.Date = DateTime.Now.Date;
            #endregion

            #region 数据库操作
            await _attendanceRepository.AddAsync(attendance);
            var succeeded = await _attendanceRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<AttendanceDto>(attendance))
                : new InternalServerErrorObjectResult("保存到数据库时遇到错误");
        }
    }
}
