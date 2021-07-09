﻿using System;
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
        private readonly LotteryResultRepository _lotteryResultRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public LotteryController(
            LotteryResultRepository lotteryResultRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _lotteryResultRepository = lotteryResultRepository
                ?? throw new ArgumentNullException(nameof(lotteryResultRepository));
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: api/values
        [HttpGet("list")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<LotteryDto>>> GetLotteryResults([FromBody] LotteryDto lotteryDto)
        {
            var user = await _userManager.FindByIdAsync(lotteryDto.WinnerId.ToString());
            if (user is null)
            {
                return BadRequest("用户不存在");
            }
            var lotteryResults = await _lotteryResultRepository.FindByUserIdAsync(lotteryDto.WinnerId);
            return Ok(_mapper.Map<LotteryDto>(lotteryResults));
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> AddLotteryResult([FromBody] LotteryDto lotteryDto)
        {
            return Ok();
        }

        // GET api/values/5
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
