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

            var lotteries = await _lotteryRepository.FindByUserIdAsync(lotteryDto.UserId.Value);
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

            #region Database operations
            var lottery = _mapper.Map<Lottery>(lotteryDto);
            await _lotteryRepository.AddAsync(lottery);
            var succeeded = await _lotteryRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<LotteryDto>(lottery))
                : new InternalServerErrorObjectResult("Update database exception");
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
