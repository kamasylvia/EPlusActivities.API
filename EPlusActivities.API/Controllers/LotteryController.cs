using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs.LotteryDtos;
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
    [Route("api/[controller]")]
    [ApiController]
    [EPlusActionFilterAttribute]
    public class LotteryController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IFindByParentIdRepository<Lottery> _lotteryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityRepository _activityRepository;
        private readonly IPrizeItemRepository _prizeItemRepository;
        private readonly IRepository<LotteryOrRedeemCount> _lotteryOrRedeemLimitRepository;
        private readonly IFindByParentIdRepository<PrizeTier> _prizeTypeRepository;

        public LotteryController(
            IFindByParentIdRepository<Lottery> lotteryRepository,
            UserManager<ApplicationUser> userManager,
            IActivityRepository activityRepository,
            IPrizeItemRepository prizeItemRepository,
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IMapper mapper,
            IRepository<LotteryOrRedeemCount> lotteryOrRedeemLimitRepository
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _lotteryOrRedeemLimitRepository =
                lotteryOrRedeemLimitRepository
                ?? throw new ArgumentNullException(nameof(lotteryOrRedeemLimitRepository));
            _lotteryRepository =
                lotteryRepository ?? throw new ArgumentNullException(nameof(lotteryRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _prizeItemRepository =
                prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _prizeTypeRepository =
                prizeTypeRepository ?? throw new ArgumentNullException(nameof(prizeTypeRepository));
        }

        // GET: api/values
        [HttpGet("user")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<LotteryDto>>> GetByUserIdAsync(
            [FromBody] LotteryForGetByUserIdDto lotteryDto
        ) {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(lotteryDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion

            var lotteries = await _lotteryRepository.FindByParentIdAsync(lotteryDto.UserId.Value);

            // 因为进行了全剧配置，AutoMapper 在此执行
            // _mapper.Map<IEnumerable<LotteryDto>>(lotteries)
            // 时会自动转换 DateTime 导致精确时间丢失，
            // 所以这里手动添加精确时间。
            var result = new List<LotteryDto>();
            foreach (var item in lotteries)
            {
                var temp = _mapper.Map<LotteryDto>(item);
                temp.Date = item.Date;
                temp.PickedTime = item.PickedUpTime;
                result.Add(temp);
            }

            return lotteries is null ? NotFound("Could not find the lottery results.") : Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<LotteryDto>> CreateAsync(
            [FromBody] LotteryForCreateDto lotteryDto
        ) {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(lotteryDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(lotteryDto.ActivityId.Value);
            if (activity is null)
            {
                return NotFound("Could not find the activity.");
            }

            if (DateTime.Today < activity.StartTime || DateTime.Today > activity.EndTime)
            {
                return BadRequest("This activity is expired.");
            }

            var count = await _lotteryOrRedeemLimitRepository.FindByIdAsync(
                lotteryDto.ActivityId.Value,
                lotteryDto.UserId.Value
            );

            if (count is null)
            {
                count = new LotteryOrRedeemCount { Activity = activity, User = user };
            }
            else if (count.Count >= activity.Limit)
            {
                return BadRequest("The user could not draw more than the limit of the activity.");
            }
            #endregion

            #region Consume the credits
            if (user.Credit < lotteryDto.UsedCredit)
            {
                return BadRequest("The user does not have enough credit.");
            }
            user.Credit -= lotteryDto.UsedCredit;
            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                return new InternalServerErrorObjectResult(updateUserResult.Errors);
            }

            count.Count++;
            if (!await _lotteryOrRedeemLimitRepository.SaveAsync())
            {
                return new InternalServerErrorObjectResult("Update database exception");
            }
            #endregion

            #region Generate the lottery result
            var lottery = _mapper.Map<Lottery>(lotteryDto);
            lottery.User = user;
            lottery.Activity = activity;
            lottery.Date = DateTime.Now;
            (lottery.PrizeTier, lottery.PrizeItem) = await RandomizeAsync(activity);
            if (lottery.PrizeTier is not null)
            {
                lottery.IsLucky = true;
            }
            #endregion

            #region Database operations
            await _lotteryRepository.AddAsync(lottery);
            var succeeded = await _lotteryRepository.SaveAsync();
            #endregion

            var result = _mapper.Map<LotteryDto>(lottery);
            result.Date = lottery.Date;

            return succeeded
                ? Ok(result)
                : new InternalServerErrorObjectResult("Update database exception");
        }

        /// <summary>
        /// 抽奖方法：
        ///     PrizeTier：几等奖。这里是按一档多奖品来写的，一档一奖也可以用，档内随机概率自动上升至 100%。
        ///     PrizeItem：奖品。
        /// </summary>
        /// <param name="activity">抽奖活动</param>
        /// <returns>(几等奖, 奖品)</returns>
        private async Task<(PrizeTier, PrizeItem)> RandomizeAsync(Activity activity)
        {
            var total = 0;
            var random = new Random();
            var flag = random.Next(100);
            var prizeTypes = activity.PrizeTiers;

            PrizeTier prizeTier = null;
            foreach (var item in prizeTypes)
            {
                total += item.Percentage;
                if (total > flag)
                {
                    prizeTier = item;
                    break;
                }
            }

            if (prizeTier is null)
            {
                return (null, null);
            }

            var prizeItems = await _prizeItemRepository.FindByPrizeTierIdAsync(prizeTier.Id.Value);
            return (prizeTier, prizeItems.ElementAtOrDefault(random.Next(prizeItems.Count())));
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateAsync([FromBody] LotteryForUpdateDto lotteryDto)
        {
            var lottery = await _lotteryRepository.FindByIdAsync(lotteryDto.Id.Value);

            #region Parameter validation
            if (lottery is null)
            {
                return NotFound("Could not find the lottery.");
            }
            #endregion

            #region Database operations
            lottery = _mapper.Map<LotteryForUpdateDto, Lottery>(lotteryDto, lottery);
            lottery.PickedUpTime = lotteryDto.PickedUpTime; // Skip auto mapper.
            _lotteryRepository.Update(lottery);
            var succeeded = await _lotteryRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpDelete]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteAsync([FromBody] LotteryForGetByIdDto lotteryDto)
        {
            var lottery = await _lotteryRepository.FindByIdAsync(lotteryDto.Id.Value);

            #region Parameter validation
            if (lottery is null)
            {
                return NotFound("Could not find the the lottery.");
            }
            #endregion

            #region Database operations
            _lotteryRepository.Remove(lottery);
            var succeeded = await _lotteryRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
