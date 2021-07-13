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
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class PrizeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFindByNameRepository<Prize> _prizeRepository;
        private readonly IMapper _mapper;

        public PrizeController(
            UserManager<ApplicationUser> userManager,
            IFindByNameRepository<Prize> prizeRepository,
            IMapper mapper)
        {
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            _prizeRepository = prizeRepository
                ?? throw new ArgumentNullException(nameof(prizeRepository));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("name")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<PrizeDto>>> GetByNameAsync([FromBody] PrizeDto prizeDto) =>
            Ok(_mapper.Map<IEnumerable<PrizeDto>>(
                await _prizeRepository.FindByNameAsync(prizeDto.Name)));

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeDto>> GetByIdAsync([FromBody] PrizeDto prizeDto)
        {
            if (prizeDto.Id == Guid.Empty)
            {
                return BadRequest("奖品 ID 不得为空");
            }

            var prize = await _prizeRepository.FindByIdAsync(prizeDto.Id);

            return prize is null ? NotFound("未找到奖品信息") : Ok(prize);
        }

        [HttpPost]
        public async Task<ActionResult<PrizeDto>> AddPrizeAsync([FromBody] PrizeDto prizeDto)
        {
            return Ok();
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
