using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs;
using EPlusActivities.API.Entities;
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
            var user = await _userManager.FindByIdAsync(lotteryDto.WinnerId.ToString());
            if (user is null)
            {
                return BadRequest("用户不存在");
            }
            var lotterys = await _lotteryRepository.FindByUserIdAsync(lotteryDto.WinnerId);
            return Ok(_mapper.Map<LotteryDto>(lotterys));
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> AddLottery([FromBody] LotteryDto lotteryDto)
        {
            return Ok();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}
