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
        private readonly IFindByParentIdRepository<PrizeType> _prizeTypeRepository;

        public LotteryController(
            IFindByParentIdRepository<Lottery> lotteryRepository,
            UserManager<ApplicationUser> userManager,
            IActivityRepository activityRepository,
            IPrizeItemRepository prizeItemRepository,
            IFindByParentIdRepository<PrizeType> prizeTypeRepository,
            IMapper mapper)
        {
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _lotteryRepository = lotteryRepository
                ?? throw new ArgumentNullException(nameof(lotteryRepository));
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            _activityRepository = activityRepository
                ?? throw new ArgumentNullException(nameof(activityRepository));
            _prizeItemRepository = prizeItemRepository
                ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _prizeTypeRepository = prizeTypeRepository
                ?? throw new ArgumentNullException(nameof(prizeTypeRepository));
        }

        // GET: api/values
        [HttpGet("user")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<LotteryDto>>> GetByUserIdAsync([FromBody] LotteryForGetByUserIdDto lotteryDto)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(lotteryDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion

            var lotteries = await _lotteryRepository.FindByParentIdAsync(lotteryDto.UserId.Value);
            return lotteries is null
                ? NotFound("Could not find the lottery results.")
                : Ok(_mapper.Map<IEnumerable<LotteryDto>>(lotteries));
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<LotteryDto>> CreateAsync([FromBody] LotteryForCreateDto lotteryDto)
        {
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

            if (lotteryDto.Date < activity.StartTime || lotteryDto.Date > activity.EndTime)
            {
                return BadRequest("This activity is expired.");
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
            #endregion

            #region Generate the lottery result
            var lottery = _mapper.Map<Lottery>(lotteryDto);
            lottery.User = user;
            lottery.Activity = activity;
            (lottery.PrizeType, lottery.PrizeItem) = await RandomizeAsync(activity);
            #endregion

            #region Database operations
            await _lotteryRepository.AddAsync(lottery);
            var succeeded = await _lotteryRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<LotteryDto>(lottery))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        /// <summary>
        /// 抽奖方法：
        ///     PrizeType：几等奖。这里是按一档多奖品来写的，一档一奖也可以用，档内随机概率自动上升至 100%。
        ///     PrizeItem：奖品。
        /// </summary>
        /// <param name="activity">抽奖活动</param>
        /// <returns>(几等奖, 奖品)</returns>
        private async Task<(PrizeType, PrizeItem)> RandomizeAsync(Activity activity)
        {
            var total = 0;
            var random = new Random();
            var flag = random.Next(100);
            var prizeTypes = activity.PrizeTypes;
            var prizeType = prizeTypes.FirstOrDefault();

            foreach (var item in prizeTypes)
            {
                if (total < flag)
                {
                    total += item.Percentage;
                }
                else
                {
                    prizeType = item;
                    break;
                }
            }

            var prizeItems = await _prizeItemRepository.FindByPrizeTypeIdAsync(prizeType.Id.Value);
            return (prizeType, prizeItems.ElementAtOrDefault(random.Next(prizeItems.Count())));
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateAsync([FromBody] LotteryForUpdateDto lotteryDto)
        {
            #region Parameter validation
            if (await _lotteryRepository.ExistsAsync(lotteryDto.Id.Value))
            {
                return NotFound("Could not find the lottery.");
            }
            #endregion

            #region Database operations
            var lottery = _mapper.Map<LotteryForUpdateDto, Lottery>(
                lotteryDto,
                await _lotteryRepository.FindByIdAsync(lotteryDto.Id.Value));
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
            #region Parameter validation
            if (await _lotteryRepository.ExistsAsync(lotteryDto.Id.Value))
            {
                return NotFound("Could not find the the lottery.");
            }
            #endregion

            #region Database operations
            var lottery = await _lotteryRepository.FindByIdAsync(lotteryDto.Id.Value);
            _lotteryRepository.Remove(lottery);
            var succeeded = await _lotteryRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
