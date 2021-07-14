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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class ActivityController : Controller
    {
        private readonly IRepository<Activity> _activityRepository;
        private readonly IMapper _mapper;
        public ActivityController(
            IRepository<Activity> activityRepository,
            IMapper mapper)
        {
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _activityRepository = activityRepository
                ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<ActivityDto>> GetByIdAsync([FromBody] ActivityDto activityDto)
        {
            #region 参数验证
            if (activityDto.Id == Guid.Empty)
            {
                return BadRequest("The activity ID could not be null");
            }
            #endregion

            var activity = await _activityRepository.FindByIdAsync(activityDto.Id);
            return activity is null
                ? NotFound("Could not find the activity")
                : Ok(_mapper.Map<ActivityDto>(activity));
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<ActivityDto>> AddActivityAsync([FromBody] ActivityDto activityDto)
        {
            #region 参数验证
            if (await _activityRepository.ExistsAsync(activityDto.Id))
            {
                return Conflict("This activity is already existed");
            }
            #endregion

            #region 数据库操作
            var activity = _mapper.Map<Activity>(activityDto);
            await _activityRepository.AddAsync(activity);
            var succeeded = await _activityRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<ActivityDto>(activity))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateActivityAsync([FromBody] ActivityDto activityDto)
        {
            #region 参数验证
            if (!await _activityRepository.ExistsAsync(activityDto.Id))
            {
                return NotFound("Could not find the activity.");
            }
            #endregion

            #region 数据库操作
            var activity = _mapper.Map<Activity>(activityDto);
            _activityRepository.Update(activity);
            var succeeded = await _activityRepository.SaveAsync();
            #endregion
            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpDelete]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteActivityAsync([FromBody] ActivityDto activityDto)
        {
            #region 参数验证
            if (!await _activityRepository.ExistsAsync(activityDto.Id))
            {
                return NotFound("Could not find the activity.");
            }
            #endregion

            #region 数据库操作
            var activity = await _activityRepository.FindByIdAsync(activityDto.Id);
            _activityRepository.Remove(activity);
            var succeeded = await _activityRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
