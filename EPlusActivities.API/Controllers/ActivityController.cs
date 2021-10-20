using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.ActivityDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.LotteryService;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 活动 API
    /// </summary>
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("choujiang/api/[controller]")]
    public class ActivityController : Controller
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IMemberService _memberService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly IFindByParentIdRepository<ActivityUser> _activityUserRepository;
        private readonly ILogger<ActivityController> _logger;
        private readonly ILotteryRepository _lotteryRepository;
        private readonly IMapper _mapper;
        private readonly IActivityService _activityService;
        private readonly ILotteryService _lotteryService;
        public ActivityController(
            IMemberService memberService,
            IActivityRepository activityRepository,
            UserManager<ApplicationUser> userManager,
            IIdGeneratorService idGeneratorService,
            IFindByParentIdRepository<ActivityUser> activityUserRepository,
            ILogger<ActivityController> logger,
            ILotteryRepository lotteryRepository,
            IMapper mapper,
            IActivityService activityService,
            ILotteryService lotteryService
        ) {
            _lotteryService =
                lotteryService ?? throw new ArgumentNullException(nameof(lotteryService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityService =
                activityService ?? throw new ArgumentNullException(nameof(activityService));
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _lotteryRepository =
                lotteryRepository ?? throw new ArgumentNullException(nameof(lotteryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="activityDto"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<ActivityDto>> GetAsync(
            [FromQuery] ActivityForGetDto activityDto
        ) {
            var activity = await _activityRepository.FindByIdAsync(activityDto.Id.Value);
            return activity is null
                ? NotFound("Could not find the activity.")
                : Ok(_mapper.Map<ActivityDto>(activity));
        }

        /// <summary>
        /// 根据活动号获取活动信息
        /// </summary>
        /// <param name="activityDto"></param>
        /// <returns></returns>
        [HttpGet("code")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<ActivityDto>> GetByActivityCodeAsync(
            [FromQuery] string activityCode
        ) {
            var activity = await _activityRepository.FindByActivityCodeAsync(activityCode);
            return activity is null
                ? NotFound("Could not find the activity.")
                : Ok(_mapper.Map<ActivityDto>(activity));
        }

        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <param name="activityDto"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivitiesAsync(
            [FromQuery] ActivityForGetListDto activityDto
        ) {
            #region Parameter validation
            if (activityDto.StartTime > activityDto.EndTime)
            {
                return BadRequest("The EndTime could not be less than the StartTime.");
            }
            #endregion

            return Ok(
                _mapper.Map<IEnumerable<ActivityDto>>(
                    activityDto.IsAvailable
                        ? await _activityService.GetAvailableActivitiesAsync(
                              activityDto.AvailableChannels.Split(
                                      new[] { ',', ';' },
                                      StringSplitOptions.TrimEntries
                                  )
                                  .Select(s => Enum.Parse<ChannelCode>(s, true)),
                              activityDto.StartTime.Value,
                              activityDto.EndTime
                          )
                        : await _activityService.GetActivitiesAsync(
                              activityDto.AvailableChannels.Split(
                                      new[] { ',', ';' },
                                      StringSplitOptions.TrimEntries
                                  )
                                  .Select(s => Enum.Parse<ChannelCode>(s, true)),
                              activityDto.StartTime.Value,
                              activityDto.EndTime
                          )
                )
            );
        }

        /// <summary>
        /// 创建活动
        /// </summary>
        /// <param name="activityDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
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

            var activityCode = _idGeneratorService.NextId().ToString().ToCharArray();
            var replacedChar = Convert.ToChar(
                Convert.ToInt32('a' + char.GetNumericValue(activityCode.FirstOrDefault()))
            );
            activityCode[0] = replacedChar;
            activity.ActivityCode = new string(activityCode);

            activity.AvailableChannels = activity.AvailableChannels.Append(ChannelCode.Test);
            await _activityRepository.AddAsync(activity);
            var succeeded = await _activityRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<ActivityDto>(activity))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        /// <summary>
        /// 修改活动信息
        /// </summary>
        /// <param name="activityDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
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

        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="activityDto"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "admin, tester"
        )]
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
            var lotteries = await _lotteryRepository.FindByActivityIdAsync(activityDto.Id.Value);
            await lotteries.ToAsyncEnumerable()
                .ForEachAsync(lottery => _lotteryRepository.Remove(lottery));
            _activityRepository.Remove(activity);
            var succeeded = await _activityRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
