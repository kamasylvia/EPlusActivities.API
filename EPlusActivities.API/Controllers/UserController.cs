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
using EPlusActivities.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        // private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        public UserController(
            UserManager<ApplicationUser> userManager,
            // SignInManager<ApplicationUser> signInManager,
            IMapper mapper)
        {
            // _signInManager = signInManager
            //     ?? throw new ArgumentNullException(nameof(signInManager));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet]
        // [Authorize(Roles = "customer")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<UserDetailsDto>> GetUserAsync([FromBody] UserLoginDto loginDto)
        {
            var user = await _userManager.FindByIdAsync(loginDto.UserId.ToString());
            return user is null
                ? NotFound("用户不存在")
                : Ok(_mapper.Map<UserDetailsDto>(user));
        }

        [HttpPatch("channel")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateLoginChannelAsync([FromBody] UserLoginDto loginDto)
        {
            #region 参数验证
            var user = await _userManager.FindByIdAsync(loginDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("用户不存在");
            }
            #endregion

            user.LoginChannel = loginDto.LoginChannel;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? Ok() : new InternalServerErrorObjectResult(result.Errors);
        }

        [HttpPatch("phonenumber")]
        // [Authorize(Roles = "test")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> ChangePhoneNumberAsync([FromBody] UserLoginDto loginDto)
        {
            #region 参数验证
            if (loginDto.PhoneNumber is null)
            {
                return BadRequest("手机号不能为空");
            }

            var user = await _userManager.FindByIdAsync(loginDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("用户不存在");
            }

            if (user.PhoneNumber == loginDto.PhoneNumber)
            {
                return Conflict("新手机号与旧手机号相同");
            }
            #endregion

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(
                user,
                loginDto.PhoneNumber);
            var result = await _userManager.ChangePhoneNumberAsync(
                user,
                loginDto.PhoneNumber,
                token);
            if (!result.Succeeded)
            {
                return new InternalServerErrorObjectResult(result.Errors);
            }

            result = await _userManager.SetUserNameAsync(user, loginDto.PhoneNumber);
            return result.Succeeded ? Ok() : new InternalServerErrorObjectResult(result.Errors);
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> AddUserAsync([FromBody] UserDetailsDto userDetailsDto)
        {
            #region 参数验证
            var user = await _userManager.FindByIdAsync(userDetailsDto.Id.ToString());
            if (user is not null)
            {
                return BadRequest("用户已存在");
            }
            #endregion
            
            // Not Completed
            user = _mapper.Map<ApplicationUser>(userDetailsDto);
            var result = await _userManager.CreateAsync(user);
            return result.Succeeded ? Ok() : new InternalServerErrorObjectResult(result.Errors);
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        // [Authorize(Roles = "customer, admin, manager")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UserDetailsDto userDetailsDto)
        {
            #region 参数验证
            var user = await _userManager.FindByIdAsync(userDetailsDto.Id.ToString());
            if (user is null)
            {
                return NotFound("用户不存在");
            }
            #endregion

            // Not Completed
            user = _mapper.Map<ApplicationUser>(userDetailsDto);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? Ok() : new InternalServerErrorObjectResult(result.Errors);
        }

        [HttpDelete]
        // [Authorize(Roles = "admin")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteUserAsync([FromBody] UserLoginDto userLoginDto)
        {
            #region 参数验证
            var user = await _userManager.FindByIdAsync(userLoginDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("用户不存在");
            }
            #endregion

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded ? Ok() : new InternalServerErrorObjectResult(result.Errors);
        }
    }
}
