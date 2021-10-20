using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 活动和用户的绑定关系 API
    /// </summary>
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("choujiang/api/[controller]")]
    public class ActivityUserController : Controller
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IMemberService _memberService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IFindByParentIdRepository<ActivityUser> _activityUserRepository;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly ILogger<ActivityUserController> _logger;
        private readonly IGeneralLotteryRecordsRepository _statementRepository;
        private readonly IActivityService _activityService;

        public ActivityUserController(
            IActivityRepository activityRepository,
            IMemberService memberService,
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<ActivityUser> activityUserRepository,
            ILogger<ActivityUserController> logger,
            IMapper mapper,
            IIdGeneratorService idGeneratorService,
            IActivityService activityService,
            IGeneralLotteryRecordsRepository statementRepository
        ) {
            _statementRepository =
                statementRepository ?? throw new ArgumentNullException(nameof(statementRepository));
            _activityService =
                activityService ?? throw new ArgumentNullException(nameof(activityService));
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

        /// <summary>
        /// 获取活动和用户的绑定关系
        /// </summary>
        /// <param name="activityUserDto"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<ActivityUserDto>> GetByIdAsync(
            [FromQuery] ActivityUserForGetDto activityUserDto
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
        /// 获取某个用户正在参与的活动
        /// </summary>
        /// <param name="activityUserDto"></param>
        /// <returns></returns>
        [HttpGet("user")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<ActivityUserDto>>> GetByUserIdAsync(
            [FromQuery] ActivityUserForGetByUserIdDto activityUserDto
        ) {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(activityUserDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion
            return Ok(
                _mapper.Map<IEnumerable<ActivityUserDto>>(
                    await _activityUserRepository.FindByParentIdAsync(activityUserDto.UserId.Value)
                )
            );
        }

        /// <summary>
        /// 绑定活动和用户，该 API 必须在签到和抽奖 API 之前被调用，否则无法判断用户参与的是哪个活动。
        /// </summary>
        /// <param name="activityUserDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "customer, tester"
        )]
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
            if (!result)
            {
                _logger.LogError("Failed to create an ActivityUser link.");
                return new InternalServerErrorObjectResult("Update database exception.");
            }
            #endregion

            return Ok();
        }

        /// <summary>
        /// 为某个用户绑定所有可参加的活动
        /// </summary>
        /// <param name="activityUserDto"></param>
        /// <returns></returns>
        [HttpPost("bindAvailable")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "customer, tester"
        )]
        public async Task<ActionResult<IEnumerable<ActivityUserDto>>> JoinAvailableActivities(
            [FromBody] ActivityUserForGetByUserIdDto activityUserDto
        ) {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(activityUserDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion

            var newCreatedLinks = await _activityService.BindUserWithAvailableActivities(
                activityUserDto.UserId.Value,
                Enum.Parse<ChannelCode>(activityUserDto.AvailableChannel, true)
            );

            return Ok();
        }

        /// <summary>
        /// 用户积分兑换抽奖次数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("redeeming")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "customer, tester"
        )]
        public async Task<ActionResult<ActivityUserForRedeemDrawsResponseDto>> RedeemDrawsAsync(
            [FromBody] ActivityUserForRedeemDrawsRequestDto request
        ) {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(request.ActivityId.Value);
            if (activity is null)
            {
                return NotFound("Could not find the activity.");
            }
            var channel = Enum.Parse<ChannelCode>(request.Channel, true);
            var generalLotteryRecords = await _statementRepository.FindByDateAsync(
                request.ActivityId.Value,
                channel,
                DateTime.Today
            );
            var requireNewStatement = generalLotteryRecords is null;
            if (requireNewStatement)
            {
                generalLotteryRecords = new GeneralLotteryRecords
                {
                    Activity = activity,
                    DateTime = DateTime.Today,
                    Channel = channel,
                };
            }

            var activityUser = await _activityUserRepository.FindByIdAsync(
                request.ActivityId.Value,
                request.UserId.Value
            );
            if (activityUser is null)
            {
                return NotFound("Could not find the ActivityUser link.");
            }

            // 今日没登陆过的用户，每日兑换次数清零
            _activityService.UpdateDailyLimitsAsync(user, activityUser);

            // 超过每日兑换限制
            if (!(activityUser.TodayUsedRedempion + request.Count <= activity.DailyRedemptionLimit))
            {
                return BadRequest(
                    "Sorry, the user had already achieved the daily maximum number of redemption of this activity."
                );
            }
            else
            {
                activityUser.TodayUsedRedempion += request.Count;
            }

            var cost = request.UnitPrice * request.Count;
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
                reason = request.Reason,
                sheetId = _idGeneratorService.NextId().ToString(),
                updateType = CreditUpdateType.Subtraction
            };
            var (updateMemberSucceed, memberForUpdateCreditResponseDto) =
                await _memberService.UpdateCreditAsync(
                    userId: request.UserId.Value,
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
                _logger.LogError(updateUserResult.ToString());
                return new InternalServerErrorObjectResult(updateUserResult.ToString());
            }
            #endregion

            #region Update ActivityUser link
            activityUser.RemainingDraws += request.Count;
            generalLotteryRecords.Redemption += request.Count;

            _activityUserRepository.Update(activityUser);
            if (requireNewStatement)
            {
                await _statementRepository.AddAsync(generalLotteryRecords);
            }
            else
            {
                _statementRepository.Update(generalLotteryRecords);
            }

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
