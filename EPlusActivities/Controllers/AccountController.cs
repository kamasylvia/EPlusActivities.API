using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs;
using EPlusActivities.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult<UserDto>> GetUserAsync([FromBody] AccountDto accountDto)
        {
            var response = await _userManager.FindByIdAsync(accountDto.UserId);

            if (response is null && accountDto.PhoneNumber is null)
            {
                return BadRequest(new ArgumentNullException("用户不存在"));
            }
            else if (response is null)
            {
                response = await _userManager.FindByNameAsync(accountDto.PhoneNumber);
            }

            return response is null
                ? BadRequest(new ArgumentNullException("用户不存在"))
                : Ok(_mapper.Map<UserDto>(response));
        }

        [HttpPatch("[Action]")]
        [Authorize(Roles = "customer, admin, manager")]
        public async Task<IActionResult> ChangePhoneNumber([FromBody] AccountDto accountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (accountDto.PhoneNumber is null)
            {
                return BadRequest(new ArgumentNullException("手机号不得为空"));
            }

            var user = await _userManager.FindByIdAsync(accountDto.UserId);
            if (user is null)
            {
                return BadRequest(new ArgumentNullException("用户不存在"));
            }

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(
                user,
                accountDto.PhoneNumber);
            var result = await _userManager.ChangePhoneNumberAsync(
                user,
                accountDto.PhoneNumber,
                token);
            return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
        }

        [HttpDelete("[Action]")]
        [Authorize(Roles = "customer, admin, manager")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UserDto userDto)
        {
            return Ok();
        }
        

        [HttpDelete("[Action]")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUserAsync([FromBody] AccountDto accountDto)
        {
            var user = await _userManager.FindByIdAsync(accountDto.UserId);
            if (user is null)
            {
                return BadRequest(new ArgumentNullException("用户不存在"));
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
        }
    }
}
