using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs.ActivityDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Enums;
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
        private readonly IActivityRepository _activityRepository;
        private readonly IMapper _mapper;
        public ActivityController(IActivityRepository activityRepository, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<ActivityDto>> GetAsync(
            [FromBody] ActivityForGetDto activityDto
        ) {
            var activity = await _activityRepository.FindByIdAsync(activityDto.Id.Value);
            return activity is null
                ? NotFound("Could not find the activity.")
                : Ok(_mapper.Map<ActivityDto>(activity));
        }

        [HttpGet("available")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetAllAvailableAsync(
            [FromBody] ActivityForGetAllAvailableDto activityDto
        ) {
            #region Parameter validation
            if (activityDto.StartTime > activityDto.EndTime)
            {
                return BadRequest("The EndTime could not be less than the StartTime.");
            }
            #endregion

            var activitiesAtStartTime = await _activityRepository.FindAllAvailableAsync(
                activityDto.StartTime.Value
            );
            var endTime = activityDto.EndTime ?? DateTime.Now.Date;
            var activitiesAtEndTime = await _activityRepository.FindAllAvailableAsync(endTime);
            var result = activitiesAtStartTime.Union(activitiesAtEndTime);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<ActivityDto>> CreateAsync(
            [FromBody] ActivityForCreateDto activityDto
        ) {
            #region Parameter validation
            if (activityDto.StartTime > activityDto.EndTime)
            {
                return BadRequest("The EndTime could not be less than the StartTime.");
            }
            #endregion

            #region Database operations
            var activity = _mapper.Map<Activity>(activityDto);
            if (
                activity.ActivityType
                    is ActivityType.SingleAttendance
                        or ActivityType.SequentialAttendance
            ) {
                activity.PrizeTiers = new List<PrizeTier>()
                {
                    new PrizeTier("Attendance") { Percentage = 100 }
                };
            }
            await _activityRepository.AddAsync(activity);
            var succeeded = await _activityRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<ActivityDto>(activity))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateAsync([FromBody] ActivityForUpdateDto activityDto)
        {
            var activity = await _activityRepository.FindByIdAsync(activityDto.Id.Value);

            #region Parameter validation
            // if (!await _activityRepository.ExistsAsync(activityDto.Id.Value))
            if (activity is null)
            {
                return NotFound("Could not find the activity.");
            }

            if (activityDto.StartTime > activityDto.EndTime)
            {
                return BadRequest("The EndTime could not be less than the StartTime.");
            }
            #endregion

            #region Database operations
            _activityRepository.Update(
                _mapper.Map<ActivityForUpdateDto, Activity>(activityDto, activity)
            );
            var succeeded = await _activityRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpDelete]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteAsync([FromBody] ActivityForGetDto activityDto)
        {
            var activity = await _activityRepository.FindByIdAsync(activityDto.Id.Value);

            #region Parameter validation
            if (activity is null)
            {
                return NotFound("Could not find the activity.");
            }
            #endregion

            #region Database operations
            _activityRepository.Remove(activity);
            var succeeded = await _activityRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
