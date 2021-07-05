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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper)
        {
            _signInManager = signInManager;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(loginDto.UserId.ToString());
            if (user is null)
            {
                return NotFound(new ArgumentException("用户不存在"));
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return await GetUserAsync(loginDto);
        }


        [HttpGet("[action]")]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult<UserDto>> GetUserAsync([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(loginDto.UserId.ToString());

            return user is null
                ? NotFound(new ArgumentException("用户不存在"))
                : Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPatch("[Action]")]
        [Authorize(Roles = "customer, admin, manager")]
        public async Task<IActionResult> ChangePhoneNumber([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userDto.PhoneNumber is null)
            {
                return BadRequest(new ArgumentException("手机号不能为空"));
            }

            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user is null)
            {
                return NotFound(new ArgumentException("用户不存在"));
            }

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(
                user,
                userDto.PhoneNumber);
            var result = await _userManager.ChangePhoneNumberAsync(
                user,
                userDto.PhoneNumber,
                token);
            return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
        }

        [HttpPost("[Action]")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> AddUserAsync([FromBody] UserDto userDto)
        {
            // Not Completed
            var user = _mapper.Map<ApplicationUser>(userDto);
            var result = await _userManager.CreateAsync(user);
            return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
        }

        [HttpPut("[Action]")]
        [Authorize(Roles = "customer, admin, manager")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UserDto userDto)
        {
            // Not Completed
            var user = _mapper.Map<ApplicationUser>(userDto);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
        }


        [HttpDelete("[Action]")]
        // [Authorize(Roles = "admin")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteUserAsync([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user is null)
            {
                return NotFound(new ArgumentException("用户不存在"));
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
        }
    }
}
