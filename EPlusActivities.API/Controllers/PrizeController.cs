using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrizeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPrizeRepository _prizeRepositor;
        private readonly IMapper _mapper;

        public PrizeController(
            UserManager<ApplicationUser> userManager,
            IPrizeRepository prizeRepositor,
            IMapper mapper)
        {
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            _prizeRepositor = prizeRepositor
                ?? throw new ArgumentNullException(nameof(prizeRepositor));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("name")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<PrizeDto>>> GetByNameAsync([FromBody] PrizeDto prizeDto)
        {
            if (string.IsNullOrEmpty(prizeDto.Name))
            {
                return BadRequest("奖品名不得为空");
            }

            var prizes = await _prizeRepositor.FindByNameAsync(prizeDto.Name);
            return Ok(_mapper.Map<IEnumerable<PrizeDto>>(prizes));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeDto>> GetByIdAsync([FromBody] PrizeDto prizeDto)
        {
            if (!prizeDto.Id.HasValue)
            {
                return BadRequest("奖品 ID 不得为空");
            }

            var prize = await _prizeRepositor.FindByIdAsync(prizeDto.Id.Value);

            return prize is null ? NotFound("未找到奖品信息") : Ok(prize);
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
