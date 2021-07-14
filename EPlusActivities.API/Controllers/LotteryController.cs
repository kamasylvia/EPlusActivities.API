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
        private readonly LotteryRepository _lotteryRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public LotteryController(
            LotteryRepository lotteryRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _lotteryRepository = lotteryRepository
                ?? throw new ArgumentNullException(nameof(lotteryRepository));
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: api/values
        [HttpGet("list")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<LotteryDto>>> GetLotteries([FromBody] LotteryDto lotteryDto)
        {
            #region 参数验证
            var user = await _userManager.FindByIdAsync(lotteryDto.UserId.ToString());
            if (user is null)
            {
                return Conflict("Could not find the user.");
            }
            #endregion

            var lotteries = await _lotteryRepository.FindByUserIdAsync(lotteryDto.UserId);
            return lotteries is null
                ? NotFound("Could not find the lottery results.")
                : Ok(_mapper.Map<IEnumerable<LotteryDto>>(lotteries));
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<LotteryDto>> AddLotteryAsync([FromBody] LotteryDto lotteryDto)
        {
            #region 参数验证
            if (await _lotteryRepository.ExistsAsync(lotteryDto.Id))
            {
                return Conflict("This lottery result is already existed.");
            }

            var user = await _userManager.FindByIdAsync(lotteryDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion

            #region 消耗积分
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
            return Ok();
        }
    }
}
