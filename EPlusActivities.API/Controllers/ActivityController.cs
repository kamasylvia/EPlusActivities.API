using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs.ActivityDtos;
using EPlusActivities.API.DTOs.ActivityUserDtos;
using EPlusActivities.API.DTOs.MemberDtos;
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
    public class ActivityController : Controller
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IMemberService _memberService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly IRepository<ActivityUser> _activityUserRepository;
        private readonly ILogger<ActivityController> _logger;
        private readonly IMapper _mapper;
        public ActivityController(
            IMemberService memberService,
            IActivityRepository activityRepository,
            UserManager<ApplicationUser> userManager,
            IIdGeneratorService idGeneratorService,
            IRepository<ActivityUser> activityUserRepository,
            ILogger<ActivityController> logger,
            IMapper mapper
        ) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        /// <summary>
        /// 绑定活动和用户，该 API 必须在签到和抽奖 API 之前被调用，否则无法判断用户参与的是哪个活动。
        /// </summary>
        /// <param name="activityUserDto"></param>
        /// <returns></returns>
        [HttpPost("user")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<ActivityUserDto>> JoinAsync(
            [FromBody] ActivityUserForJoinDto activityUserDto
        ) {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(activityUserDto.UserId.Value.ToString());
            if (user is null)
            {
                return BadRequest("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(
                activityUserDto.ActivityId.Value
            );
            if (activity is null)
            {
                return BadRequest("Could not find the activity.");
            }

            var activityUser = await _activityUserRepository.FindByIdAsync(
                activityUserDto.ActivityId.Value,
                activityUserDto.UserId.Value
            );
            if (activityUser is not null)
            {
                return Conflict("The user had already joined the activity.");
            }
            #endregion

            #region Create an ActivityUser link
            activityUser = new ActivityUser { Activity = activity, User = user, };
            #endregion

            #region Database operations
            await _activityUserRepository.AddAsync(activityUser);
            var result = await _activityUserRepository.SaveAsync();
            if (result)
            {
                return Ok();
            }
            #endregion

            _logger.LogError("Failed to create an ActivityUser link.");
            return new InternalServerErrorObjectResult("Update database exception.");
        }

        [HttpPatch("redeeming")]
        // [Authorize(Roles = "test")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<ActivityUserForRedeemDrawsResponseDto>> RedeemDrawsAsync(
            [FromBody] ActivityUserForRedeemDrawsRequestDto activityUserDto
        ) {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(activityUserDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(
                activityUserDto.ActivityId.Value
            );
            if (activity is null)
            {
                return NotFound("Could not find the activity.");
            }

            var activityUser = await _activityUserRepository.FindByIdAsync(
                activityUserDto.ActivityId.Value,
                activityUserDto.UserId.Value
            );
            if (activityUser is null)
            {
                return NotFound("Could not find the ActivityUser link.");
            }

            var cost = activityUserDto.UnitPrice * activityUserDto.Count;
            if (cost > user.Credit)
            {
                return BadRequest("The user did not have enough credits.");
            }
            #endregion

            #region Connect member server
            var (getMemberSucceed, member) = await _memberService.GetMemberAsync(user.PhoneNumber);
            var memberForUpdateCreditRequestDto = new MemberForUpdateCreditRequestDto
            {
                memberId = member.Body.Content.MemberId,
                points = user.Credit,
                reason = activityUserDto.Reason,
                sheetId = _idGeneratorService.NextId().ToString(),
                updateType = CreditUpdateType.Subtraction
            };
            var (updateMemberSucceed, memberForUpdateCreditResponseDto) =
                await _memberService.UpdateCreditAsync(
                activityUserDto.UserId.Value,
                memberForUpdateCreditRequestDto
            );
            #endregion

            #region Update credit
            if (!updateMemberSucceed)
            {
                var error = "Failed to update the credit.";
                _logger.LogError(error);
                return new InternalServerErrorObjectResult(error);
            }

            user.Credit = memberForUpdateCreditResponseDto.Body.Content.NewPoints;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to update the user.");
                return new InternalServerErrorObjectResult(result.Errors);
            }
            #endregion

            return Ok(_mapper.Map<ActivityUserForRedeemDrawsResponseDto>(activityUser));
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
