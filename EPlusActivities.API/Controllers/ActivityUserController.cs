using System;
using System.Threading.Tasks;
using AutoMapper;
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

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class ActivityUserController : Controller
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IMemberService _memberService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IRepository<ActivityUser> _activityUserRepository;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly ILogger<ActivityUserController> _logger;

        public ActivityUserController(
            IActivityRepository activityRepository,
            IMemberService memberService,
            UserManager<ApplicationUser> userManager,
            IRepository<ActivityUser> activityUserRepository,
            ILogger<ActivityUserController> logger,
            IMapper mapper,
            IIdGeneratorService idGeneratorService
        ) {
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<ActivityUserDto>> GetByIdAsync(
            [FromBody] ActivityUserForGetDto activityUserDto
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
            #endregion

            var activityUser = await _activityUserRepository.FindByIdAsync(
                activityUserDto.ActivityId.Value,
                activityUserDto.UserId.Value
            );

            return activityUser is null
                ? NotFound("Could not find the ActivityUser link.")
                : Ok(_mapper.Map<ActivityUserDto>(activityUser));
        }

        /// <summary>
        /// 绑定活动和用户，该 API 必须在签到和抽奖 API 之前被调用，否则无法判断用户参与的是哪个活动。
        /// </summary>
        /// <param name="activityUserDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<ActivityUserDto>> JoinAsync(
            [FromBody] ActivityUserForGetDto activityUserDto
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
                points = cost,
                reason = activityUserDto.Reason,
                sheetId = _idGeneratorService.NextId().ToString(),
                updateType = CreditUpdateType.Subtraction
            };
            var (updateMemberSucceed, memberForUpdateCreditResponseDto) =
                await _memberService.UpdateCreditAsync(
                userId: activityUserDto.UserId.Value,
                requestDto: memberForUpdateCreditRequestDto
            );
            #endregion

            #region Update the user's credit
            if (!updateMemberSucceed)
            {
                var error = "Failed to update the credit.";
                _logger.LogError(error);
                return new InternalServerErrorObjectResult(error);
            }

            user.Credit = memberForUpdateCreditResponseDto.Body.Content.NewPoints;
            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                _logger.LogError("Failed to update the user.");
                return new InternalServerErrorObjectResult(updateUserResult.Errors);
            }
            #endregion

            #region Update ActivityUser link
            activityUser.RemainingDraws =
                activityUser.RemainingDraws + activityUserDto.Count ?? activityUserDto.Count;
            _activityUserRepository.Update(activityUser);
            var updateActivityUserResult = await _activityUserRepository.SaveAsync();
            if (!updateActivityUserResult)
            {
                _logger.LogError("Failed to update the ActivityUser link");
                return new InternalServerErrorObjectResult("Update database exception");
            }
            #endregion

            return Ok(_mapper.Map<ActivityUserForRedeemDrawsResponseDto>(activityUser));
        }
    }
}
